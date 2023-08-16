using Woa.Sdk.Wechat;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Location)]
public class WechatLocationMessageHandler : WechatUserMessageHandler
{
	public WechatLocationMessageHandler(IWechatUserMessageStore store)
		: base(store)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}