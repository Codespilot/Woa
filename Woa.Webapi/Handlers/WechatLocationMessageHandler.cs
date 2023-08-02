using Woa.Webapi.Models;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Location)]
public class WechatLocationMessageHandler : WechatUserMessageHandler
{
    public WechatLocationMessageHandler(SupabaseClient client, ILoggerFactory logger)
        : base(client, logger)
    {
    }

    protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}