using System.Text;

namespace Woa.Common;

public static class StringExtension
{
	public static string Hex(this string text, string separator = null)
	{
		var builder = new StringBuilder();
		foreach (var chr in text)
		{
			builder.Append($"{Convert.ToInt32(chr):X2}");
			builder.Append(separator ?? string.Empty);
		}

		return builder.ToString();
	}

	public static string Mask(this string source, int start, int length, char maskChar = '*')
	{
		var end = start + length;
		if (source.Length <= start)
		{
			return string.Empty;
		}

		if (source.Length < end)
		{
			return source[..start] + "".PadLeft(source.Length - start, maskChar);
		}

		return source[..start] + "".PadLeft(length, maskChar) + source[end..];
	}

	public static string GetValueOrDefault(this string source, string defaultValue = null)
	{
		return string.IsNullOrEmpty(source) ? defaultValue : source;
	}
}