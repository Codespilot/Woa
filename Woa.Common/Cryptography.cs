using System.Security.Cryptography;
using System.Text;

namespace Woa.Common;

public class Cryptography
{
	// ReSharper disable once InconsistentNaming
	public class RSA
	{
		private readonly System.Security.Cryptography.RSA _privateKeyRsaProvider;
		private readonly System.Security.Cryptography.RSA _publicKeyRsaProvider;

		/// <summary>
		/// 实例化RSAHelper
		/// </summary>
		/// <param name="privateKey">私钥</param>
		/// <param name="publicKey">公钥</param>
		public RSA(string privateKey, string publicKey = null)
		{
			if (!string.IsNullOrEmpty(privateKey))
			{
				_privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
			}

			if (!string.IsNullOrEmpty(publicKey))
			{
				_publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
			}
		}

		public string Encrypt(string source)
		{
			if (_publicKeyRsaProvider == null)
			{
				throw new Exception("_publicKeyRsaProvider is null");
			}

			return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(source), RSAEncryptionPadding.Pkcs1));
		}

		public string Decrypt(string source)
		{
			if (_privateKeyRsaProvider == null)
			{
				throw new Exception("_privateKeyRsaProvider is null");
			}

			return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(Convert.FromBase64String(source), RSAEncryptionPadding.Pkcs1));
		}

		private static System.Security.Cryptography.RSA CreateRsaProviderFromPrivateKey(string key)
		{
			var privateKeyBits = Convert.FromBase64String(key);

			var rsa = System.Security.Cryptography.RSA.Create();
			var parameters = new RSAParameters();

			using (var reader = new BinaryReader(new MemoryStream(privateKeyBits)))
			{
				var twobytes = reader.ReadUInt16();
				switch (twobytes)
				{
					case 0x8130:
						reader.ReadByte();
						break;
					case 0x8230:
						reader.ReadInt16();
						break;
					default:
						throw new Exception("Unexpected value read reader.ReadUInt16()");
				}

				twobytes = reader.ReadUInt16();
				if (twobytes != 0x0102)
				{
					throw new Exception("Unexpected version");
				}

				var bt = reader.ReadByte();
				if (bt != 0x00)
				{
					throw new Exception("Unexpected value read reader.ReadByte()");
				}

				parameters.Modulus = reader.ReadBytes(GetIntegerSize(reader));
				parameters.Exponent = reader.ReadBytes(GetIntegerSize(reader));
				parameters.D = reader.ReadBytes(GetIntegerSize(reader));
				parameters.P = reader.ReadBytes(GetIntegerSize(reader));
				parameters.Q = reader.ReadBytes(GetIntegerSize(reader));
				parameters.DP = reader.ReadBytes(GetIntegerSize(reader));
				parameters.DQ = reader.ReadBytes(GetIntegerSize(reader));
				parameters.InverseQ = reader.ReadBytes(GetIntegerSize(reader));
			}

			rsa.ImportParameters(parameters);
			return rsa;
		}

		private static System.Security.Cryptography.RSA CreateRsaProviderFromPublicKey(string key)
		{
			byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

			var x509Key = Convert.FromBase64String(key);

			using (var mem = new MemoryStream(x509Key))
			{
				using (var reader = new BinaryReader(mem))
				{
					var twobytes = reader.ReadUInt16();
					switch (twobytes)
					{
						case 0x8130:
							reader.ReadByte();
							break;
						case 0x8230:
							reader.ReadInt16();
							break;
						default:
							return null;
					}

					var seq = reader.ReadBytes(15);
					if (!CompareBytes(seq, seqOid)) //make sure Sequence for OID is correct
					{
						return null;
					}

					twobytes = reader.ReadUInt16();
					switch (twobytes)
					{
						//data read as little endian order (actual data order for Bit String is 03 81)
						case 0x8103:
							reader.ReadByte(); //advance 1 byte
							break;
						case 0x8203:
							reader.ReadInt16(); //advance 2 bytes
							break;
						default:
							return null;
					}

					var bt = reader.ReadByte();
					if (bt != 0x00) //expect null byte next
						return null;

					twobytes = reader.ReadUInt16();
					switch (twobytes)
					{
						//data read as little endian order (actual data order for Sequence is 30 81)
						case 0x8130:
							reader.ReadByte(); //advance 1 byte
							break;
						case 0x8230:
							reader.ReadInt16(); //advance 2 bytes
							break;
						default:
							return null;
					}

					twobytes = reader.ReadUInt16();
					byte lowByte;
					byte highByte = 0x00;

					switch (twobytes)
					{
						//data read as little endian order (actual data order for Integer is 02 81)
						case 0x8102:
							lowByte = reader.ReadByte(); // read next bytes which is bytes in modulus
							break;
						case 0x8202:
							highByte = reader.ReadByte(); //advance 2 bytes
							lowByte = reader.ReadByte();
							break;
						default:
							return null;
					}

					byte[] value = { lowByte, highByte, 0x00, 0x00 }; //reverse byte order since asn.1 key uses big endian order
					var modulusSize = BitConverter.ToInt32(value, 0);

					int firstbyte = reader.PeekChar();
					if (firstbyte == 0x00)
					{
						//if first byte (highest order) of modulus is zero, don't include it
						reader.ReadByte(); //skip this null byte
						modulusSize -= 1; //reduce modulus buffer size by 1
					}

					var modulus = reader.ReadBytes(modulusSize);

					if (reader.ReadByte() != 0x02)
					{
						return null;
					}

					int exponentCount = reader.ReadByte();
					var exponent = reader.ReadBytes(exponentCount);

					// ------- create RSACryptoServiceProvider instance and initialize with public key -----
					var rsa = System.Security.Cryptography.RSA.Create();
					var rsaKeyInfo = new RSAParameters
					{
						Modulus = modulus,
						Exponent = exponent
					};
					rsa.ImportParameters(rsaKeyInfo);

					return rsa;
				}
			}
		}

		private static int GetIntegerSize(BinaryReader reader)
		{
			int count;
			var @byte = reader.ReadByte();
			if (@byte != 0x02)
			{
				return 0;
			}

			@byte = reader.ReadByte();

			switch (@byte)
			{
				case 0x81:
					count = reader.ReadByte();
					break;
				case 0x82:
					{
						var highByte = reader.ReadByte();
						var lowByte = reader.ReadByte();
						byte[] value = { lowByte, highByte, 0x00, 0x00 };
						count = BitConverter.ToInt32(value, 0);
						break;
					}
				default:
					count = @byte;
					break;
			}

			while (reader.ReadByte() == 0x00)
			{
				count -= 1;
			}

			reader.BaseStream.Seek(-1, SeekOrigin.Current);
			return count;
		}

		private static bool CompareBytes(byte[] source, byte[] target)
		{
			if (source.Length != target.Length)
			{
				return false;
			}

			var i = 0;
			foreach (var @byte in source)
			{
				if (@byte != target[i])
				{
					return false;
				}

				i++;
			}

			return true;
		}
	}

	// ReSharper disable once InconsistentNaming
	public class DES
	{
		/// <summary>
		/// 32位Key值：
		/// </summary>
		private static readonly byte[] _defaultSalt = { 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x9B, 0x93, 0x8B, 0x83, 0x7B, 0x73, 0x6B, 0x63, 0x5B, 0x53, 0xF3, 0xFB, 0xA3, 0xAB, 0xB3, 0xBB, 0xC3, 0xEB, 0xE3, 0xDB, 0xD3, 0xCB };

		#region DES加密

		/// <summary>
		/// DES加密
		/// </summary>
		/// <param name="source">待加密字串</param>
		/// <returns>加密后的字符串</returns>
		public static string Encrypt(string source)
		{
			return Encrypt(source, _defaultSalt);
		}

		/// <summary>
		/// DES加密
		/// </summary>
		/// <param name="source">待加密字串</param>
		/// <param name="key">Key值</param>
		/// <returns>加密后的字符串</returns>
		public static string Encrypt(string source, byte[] key)
		{
			using SymmetricAlgorithm sa = Aes.Create(); //Rijndael.Create();
			sa.Key = key;
			sa.Mode = CipherMode.ECB;
			sa.Padding = PaddingMode.PKCS7;
			using var ms = new MemoryStream();
			using var cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write);
			var byt = Encoding.Unicode.GetBytes(source);
			cs.Write(byt, 0, byt.Length);
			cs.FlushFinalBlock();
			cs.Close();
			return Convert.ToBase64String(ms.ToArray());
		}

		public static string Encrypt(string source, string key)
		{
			using var provider = System.Security.Cryptography.DES.Create();
			{
				provider.Mode = CipherMode.ECB;
				provider.Padding = PaddingMode.PKCS7;
			}

			var keyBytes = Encoding.UTF8.GetBytes(key);
			var sourceBytes = Encoding.UTF8.GetBytes(source);
			using var memory = new MemoryStream();
			using var crypto = new CryptoStream(memory, provider.CreateEncryptor(keyBytes, keyBytes), CryptoStreamMode.Write);
			crypto.Write(sourceBytes, 0, sourceBytes.Length);
			crypto.FlushFinalBlock();
			return Convert.ToBase64String(memory.ToArray());
		}

		#endregion

		#region DES解密

		/// <summary>
		/// DES解密
		/// </summary>
		/// <param name="source">待解密的字串</param>
		/// <returns>解密后的字符串</returns>
		public static string Decrypt(string source)
		{
			return Decrypt(source, _defaultSalt);
		}

		/// <summary>
		/// DES解密
		/// </summary>
		/// <param name="source">待解密的字串</param>
		/// <param name="key">32位Key值</param>
		/// <returns>解密后的字符串</returns>
		public static string Decrypt(string source, byte[] key)
		{
			using SymmetricAlgorithm sa = Aes.Create(); //Rijndael.Create();
			sa.Key = key;
			sa.Mode = CipherMode.ECB;
			sa.Padding = PaddingMode.PKCS7;
			var ct = sa.CreateDecryptor();
			var byt = Convert.FromBase64String(source);
			using var ms = new MemoryStream(byt);
			using var cs = new CryptoStream(ms, ct, CryptoStreamMode.Read);
			using var sr = new StreamReader(cs, Encoding.Unicode);
			return sr.ReadToEnd();
		}

		#endregion
	}

	// ReSharper disable once InconsistentNaming
	public class AES
	{
		private static readonly byte[] _defaultSalt = { 0x03, 0x0B, 0x13, 0x1B, 0x23, 0x2B, 0x33, 0x3B, 0x43, 0x4B, 0x9B, 0x93, 0x8B, 0x83, 0x7B, 0x73, 0x6B, 0x63, 0x5B, 0x53, 0xF3, 0xFB, 0xA3, 0xAB, 0xB3, 0xBB, 0xC3, 0xEB, 0xE3, 0xDB, 0xD3, 0xCB };

		/// <summary>
		///  AES 加密
		/// </summary>
		/// <param name="source">待加密文本</param>
		/// <returns></returns>
		public static string Encrypt(string source)
		{
			return Encrypt(source, _defaultSalt);
		}

		/// <summary>
		///  AES 加密
		/// </summary>
		/// <param name="source">待加密文本</param>
		/// <param name="key">密钥</param>
		/// <returns></returns>
		public static string Encrypt(string source, string key)
		{
			return Encrypt(source, Encoding.UTF8.GetBytes(key));
		}

		/// <summary>
		///  AES 加密
		/// </summary>
		/// <param name="source">待加密文本</param>
		/// <param name="key">密钥</param>
		/// <returns></returns>
		public static string Encrypt(string source, byte[] key)
		{
			if (string.IsNullOrEmpty(source))
			{
				return null;
			}

			var bytes = Encoding.UTF8.GetBytes(source);

			using (var rm = Aes.Create())
			{
				rm.Key = key;
				rm.Mode = CipherMode.ECB;
				rm.Padding = PaddingMode.PKCS7;
				var encryptor = rm.CreateEncryptor();
				var result = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
				return Convert.ToBase64String(result);
			}
		}

		/// <summary>
		///  AES 解密
		/// </summary>
		/// <param name="source">待解密文本</param>
		/// <returns></returns>
		public static string Decrypt(string source)
		{
			return Decrypt(source, _defaultSalt);
		}

		/// <summary>
		///  AES 解密
		/// </summary>
		/// <param name="source">待解密文本</param>
		/// <param name="key">密钥</param>
		/// <returns></returns>
		public static string Decrypt(string source, string key)
		{
			return Decrypt(source, Encoding.UTF8.GetBytes(key));
		}

		/// <summary>
		///  AES 解密
		/// </summary>
		/// <param name="source">待解密文本</param>
		/// <param name="key">密钥</param>
		/// <returns></returns>
		public static string Decrypt(string source, byte[] key)
		{
			if (string.IsNullOrEmpty(source))
			{
				return null;
			}

			var bytes = Convert.FromBase64String(source);

			using (var rm = Aes.Create())
			{
				rm.Key = key;
				rm.Mode = CipherMode.ECB;
				rm.Padding = PaddingMode.PKCS7;

				var decryptor = rm.CreateDecryptor();
				var result = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

				return Encoding.UTF8.GetString(result);
			}
		}
	}

	/// <summary>
	/// MD5加密
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class MD5
	{
		/// <summary>
		/// MD5函数,需引用：using System.Security.Cryptography;
		/// </summary>
		/// <param name="value">原始字符串</param>
		/// <returns>MD5结果</returns>
		public static string Encrypt(string value)
		{
#if NETSTANDARD2_1
			using var md5 = System.Security.Cryptography.MD5.Create();
			var bytes = md5.ComputeHash(Encoding.Default.GetBytes(value));
#elif NET6_0_OR_GREATER
			var bytes = System.Security.Cryptography.MD5.HashData(Encoding.Default.GetBytes(value));
#endif
			var builder = new StringBuilder();
			foreach (var @byte in bytes)
			{
				builder.Append(@byte.ToString("x").PadLeft(2, '0'));
			}

			return builder.ToString();
		}
	}

	public class Base64
	{ 
		public static string Encrypt(string source)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
		}

		public static string Decrypt(string source)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(source));
		}
	}
}