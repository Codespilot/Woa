#pragma warning disable CA1050
public class Check
{
	public static void Ensure<T>(T value, Func<T, bool> action, Action<T> nextAction, Action failsAction = null)
	{
		var result = action(value);
		if (result)
		{
			nextAction?.Invoke(value);
		}
		else
		{
			failsAction?.Invoke();
		}
	}

	public static T Ensure<T>(T value, Func<T, bool> action, T defaultValue)
	{
		var result = action(value);
		return result ? value : defaultValue;
	}
}
#pragma warning restore CA1050