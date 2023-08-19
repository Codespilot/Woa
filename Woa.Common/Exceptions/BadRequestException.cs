using System.Runtime.Serialization;

namespace Woa.Common;

public sealed class BadRequestException : Exception
{
	public BadRequestException()
	{
	}

	public BadRequestException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public BadRequestException(string message)
		: base(message)
	{
	}

	public BadRequestException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}