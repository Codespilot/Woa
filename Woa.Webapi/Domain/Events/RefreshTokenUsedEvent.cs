using MediatR;

namespace Woa.Webapi.Domain;

public class RefreshTokenUsedEvent : INotification
{
	public RefreshTokenUsedEvent(string token)
	{
		Token = token;
	}

	public string Token { get; set; }
}