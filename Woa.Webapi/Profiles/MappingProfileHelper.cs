using System.Globalization;
using System.Text.RegularExpressions;

namespace Woa.Webapi.Profiles;

internal class MappingProfileHelper
{
	public static string TrimString(string value, bool normalize = true)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}

		value = Regex.Replace(value, @"\s+", string.Empty);
		return normalize ? CultureInfo.CurrentCulture.TextInfo.ToLower(value) : value;
	}

	public static string[] Split(string value, string separator)
	{
		return string.IsNullOrEmpty(value) ? Array.Empty<string>() : value.Split(separator);
	}

	public static string Splice(IEnumerable<string> value, string separator)
	{
		return string.Join(separator, value);
	}
}