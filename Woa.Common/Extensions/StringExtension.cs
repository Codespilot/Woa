using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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

	public static string Trim(this string source, TextTrimType type)
	{
		if (string.IsNullOrEmpty(source))
		{
			return source;
		}

		return type switch
		{
			TextTrimType.Head => source.TrimStart(),
			TextTrimType.Tail => source.TrimEnd(),
			TextTrimType.Both => source.Trim(),
			TextTrimType.All => Regex.Replace(source, @"\s+", string.Empty),
			TextTrimType.None => source,
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}

	public static string Normalize(this string source, TextCaseType caseType)
	{
		if (string.IsNullOrEmpty(source))
		{
			return source;
		}

		var text = CultureInfo.CurrentCulture.TextInfo;
		return caseType switch
		{
			TextCaseType.Upper => text.ToUpper(source),
			TextCaseType.Lower => text.ToLower(source),
			TextCaseType.Title => text.ToTitleCase(source),
			TextCaseType.None => source,
			_ => throw new ArgumentOutOfRangeException(nameof(caseType), caseType, null)
		};
	}
}