namespace Woa.Common;

public sealed class ValidationException : Exception
{
	public ValidationException(IReadOnlyDictionary<string, string[]> errors)
		: base("验证失败")
	{
		Errors = errors;
	}

	public IReadOnlyDictionary<string, string[]> Errors { get; }
}