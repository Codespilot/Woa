namespace Woa.Webapi.Domain;

public class WechatAccountSetValidityCommand : ICommand
{
	public WechatAccountSetValidityCommand(string id, bool validity)
	{
		Id = id;
		Validity = validity;
	}

	public string Id { get; }

	public bool Validity { get; }
}