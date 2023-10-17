namespace Woa.Common;

public class GenericEqualityComparer<T> : IEqualityComparer<T>
{
	private readonly Func<T, int> _hashCodeFunc;
	private readonly Func<T, T, bool> _compareFunc;

	public GenericEqualityComparer(Func<T, T, bool> compareFunc, Func<T, int> hashCodeFunc)
	{
		_compareFunc = compareFunc;
		_hashCodeFunc = hashCodeFunc;
	}

	public bool Equals(T x, T y)
	{
		return _compareFunc(x, y);
	}

	public int GetHashCode(T obj)
	{
		return _hashCodeFunc?.Invoke(obj) ?? obj.GetHashCode();
	}
}