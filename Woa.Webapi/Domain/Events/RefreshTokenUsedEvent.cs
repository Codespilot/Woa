namespace Woa.Webapi.Domain;

public class RefreshTokenUsedEvent : IEvent
{
	public RefreshTokenUsedEvent(string token)
	{
		Token = token;
	}

	public string Token { get; set; }
}