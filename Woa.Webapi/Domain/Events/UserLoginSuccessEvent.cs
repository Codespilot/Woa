using MediatR;

namespace Woa.Webapi.Domain;

public class UserLoginSuccessEvent : INotification
{
	public UserLoginSuccessEvent(int userId)
	{
		UserId = userId;
	}

	public int UserId { get; set; }
}