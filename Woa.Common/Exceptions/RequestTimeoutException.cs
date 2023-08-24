using System.Runtime.Serialization;

namespace Woa.Common;

public class RequestTimeoutException : Exception
{
	public RequestTimeoutException()
	{
	}

	protected RequestTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public RequestTimeoutException(string message)
		: base(message)
	{
	}

	public RequestTimeoutException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}