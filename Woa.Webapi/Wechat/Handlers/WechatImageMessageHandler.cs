using Woa.Sdk.Wechat;

namespace Woa.Webapi.Wechat;

[WechatMessageHandle(WechatMessageType.Image)]
public class WechatImageMessageHandler : WechatUserMessageHandler
{
	public WechatImageMessageHandler(IWechatUserMessageStore client)
		: base(client)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		return await Task.FromResult(WechatMessage.Empty);
	}
}