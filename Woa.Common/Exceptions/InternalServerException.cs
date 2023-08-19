using System.Runtime.Serialization;

namespace Woa.Common;

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