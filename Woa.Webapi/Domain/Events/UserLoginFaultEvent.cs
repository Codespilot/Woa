using MediatR;

namespace Woa.Webapi.Domain;

public class UserLoginFaultEvent : IEvent
{
	public UserLoginFaultEvent(long userId)
	{
		UserId = userId;
	}

	public long UserId { get; }
}