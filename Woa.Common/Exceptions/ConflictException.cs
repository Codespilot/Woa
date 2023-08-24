﻿using System.Runtime.Serialization;

namespace Woa.Common;

/// <summary>
///   409 Conflict
/// </summary>
public class ConflictException : Exception
{
	public ConflictException()
	{
	}

	protected ConflictException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}

	public ConflictException(string message)
		: base(message)
	{
	}

	public ConflictException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}