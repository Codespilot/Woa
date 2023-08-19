using System.Data;
using System.Runtime.Serialization;

namespace Woa.Common;

public class NotFoundException : DataException
{
	public NotFoundException()
	{
	}

	protected NotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public NotFoundException(string message)
		: base(message)
	{
	}

	public NotFoundException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}