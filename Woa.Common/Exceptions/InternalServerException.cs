using System.Runtime.Serialization;

namespace Woa.Common;

/// <summary>
/// 服务器内部错误异常
/// </summary>
public class InternalServerException : Exception
{
	public InternalServerException()
	{
	}

	protected InternalServerException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public InternalServerException(string message)
		: base(message)
	{
	}

	public InternalServerException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}