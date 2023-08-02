using Woa.Webapi.Models;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Image)]
public class WechatImageMessageHandler : WechatUserMessageHandler
{
    public WechatImageMessageHandler(SupabaseClient client, ILoggerFactory logger)
        : base(client, logger)
    {
    }

    protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
    {
        return WechatMessage.Empty;
    }
}