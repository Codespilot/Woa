using Woa.Webapi.Models;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Link)]
public class WechatLinkMessageHandler : WechatUserMessageHandler
{
    public WechatLinkMessageHandler(SupabaseClient client, ILoggerFactory logger)
        : base(client, logger)
    {
    }

    protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}