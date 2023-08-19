using MediatR;

namespace Woa.Webapi.Domain;

public class UserLoginSuccessEvent : IEvent
{
	public UserLoginSuccessEvent(int userId)
	{
		UserId = userId;
	}

	public int UserId { get; set; }
}