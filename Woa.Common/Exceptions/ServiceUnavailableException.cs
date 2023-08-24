using System.Runtime.Serialization;

namespace Woa.Common;

/// <summary>
/// 服务不可用异常
/// </summary>
public class ServiceUnavailableException : Exception
{
	/// <inheritdoc />
	public ServiceUnavailableException()
	{
	}

	/// <inheritdoc />
	protected ServiceUnavailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	/// <inheritdoc />
	public ServiceUnavailableException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public ServiceUnavailableException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}