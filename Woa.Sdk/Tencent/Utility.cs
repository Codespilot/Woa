using System.Security.Cryptography;
using System.Text;

namespace Woa.Sdk.Tencent;

public static class Utility
{
    public static bool VerifySignature(string signature, string timestamp, string nonce, string token)
	{
		var parameters = new List<string> { token, timestamp, nonce };
		parameters.Sort();

		var res = string.Join("", parameters);

		var asciiBytes = Encoding.ASCII.GetBytes(res);
		var hashBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")!).ComputeHash(asciiBytes);
		var builder = new StringBuilder();
		foreach (var @byte in hashBytes)
		{
			builder.Append(@byte.ToString("x2"));
		}

		var result = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
		return string.Equals(result, signature, StringComparison.OrdinalIgnoreCase);
	}
}