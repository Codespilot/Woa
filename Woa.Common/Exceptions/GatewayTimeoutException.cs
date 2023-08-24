using System.Runtime.Serialization;

namespace Woa.Common;

/// <summary>
/// 网关超时异常
/// </summary>
public class GatewayTimeoutException : Exception
{
	/// <inheritdoc />
	public GatewayTimeoutException()
	{
	}

	/// <inheritdoc />
	protected GatewayTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	/// <inheritdoc />
	public GatewayTimeoutException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public GatewayTimeoutException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}