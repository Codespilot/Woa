using MediatR;

namespace Woa.Webapi.Domain;

public class UserLoginFaultEvent : IEvent
{
	public UserLoginFaultEvent(int userId)
	{
		UserId = userId;
	}

	public int UserId { get; }
}