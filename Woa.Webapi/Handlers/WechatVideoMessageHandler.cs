using Woa.Webapi.Models;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Video), WechatMessageHandle(WechatMessageType.ShortVideo)]
public class WechatVideoMessageHandler : WechatUserMessageHandler
{
    public WechatVideoMessageHandler(SupabaseClient client, ILoggerFactory logger)
        : base(client, logger)
    {
    }

    protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}