namespace Woa.Common;

public static class DictionaryExtensions
{
	public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
	{
		return source.TryGetValue(key, out var value) ? value : default;
	}

	public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
	{
		if (source == null)
		{
			return defaultValue;
		}

		return source.TryGetValue(key, out var value) ? value : defaultValue;
	}
}