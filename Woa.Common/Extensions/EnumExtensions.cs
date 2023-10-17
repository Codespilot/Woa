using System.ComponentModel;
using System.Reflection;

namespace Woa.Common;

public static class EnumExtensions
{
	public static IEnumerable<TEnum> GetFlags<TEnum>(this Enum value)
		where TEnum : Enum
	{
		foreach (Enum item in Enum.GetValues(value.GetType()))
		{
			if (value.HasFlag(item))
			{
				yield return (TEnum)item;
			}
		}
	}

	/// <summary>
	/// 获取枚举描述
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static string GetDescription(this Enum value)
	{
		var field = value.GetType().GetField(value.ToString());
		if (field == null)
		{
			throw new NullReferenceException("field");
		}
		var attribute = field.GetCustomAttribute<DescriptionAttribute>();
		return attribute?.Description ?? value.ToString();
	}
}