using Woa.Sdk.Wechat;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Image)]
public class WechatImageMessageHandler : WechatUserMessageHandler
{
	public WechatImageMessageHandler(IWechatUserMessageStore client)
		: base(client)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
	{
		return WechatMessage.Empty;
	}
}