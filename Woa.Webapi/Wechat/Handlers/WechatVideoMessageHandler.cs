using Woa.Sdk.Wechat;

namespace Woa.Webapi.Wechat;

[WechatMessageHandle(WechatMessageType.Video), WechatMessageHandle(WechatMessageType.ShortVideo)]
public class WechatVideoMessageHandler : WechatUserMessageHandler
{
	public WechatVideoMessageHandler(IWechatUserMessageStore store)
		: base(store)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}