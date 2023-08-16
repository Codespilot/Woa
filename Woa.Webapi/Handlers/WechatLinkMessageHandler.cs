using Woa.Sdk.Wechat;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Link)]
public class WechatLinkMessageHandler : WechatUserMessageHandler
{
    public WechatLinkMessageHandler(IWechatUserMessageStore store)
        : base(store)
    {
    }

    protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}