using System.Runtime.Serialization;

namespace Woa.Common;

/// <summary>
///    502 Bad Gateway
/// </summary>
public class BadGatewayException : Exception
{
	public BadGatewayException()
	{
	}

	protected BadGatewayException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public BadGatewayException(string message)
		: base(message)
	{
	}

	public BadGatewayException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}