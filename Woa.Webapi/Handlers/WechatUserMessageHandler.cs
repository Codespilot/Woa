using Postgrest;
using Woa.Webapi.Entities;
using Woa.Webapi.Models;

namespace Woa.Webapi.Handlers;

public abstract class WechatUserMessageHandler : IWechatMessageHandler
{
    private readonly SupabaseClient _client;

    protected WechatUserMessageHandler(SupabaseClient client, ILoggerFactory logger)
    {
        _client = client;
        Logger = logger.CreateLogger(GetType());
    }

    protected virtual SupabaseClient SupabaseClient => _client;

    protected virtual ILogger Logger { get; }

    public Task<WechatMessage> HandleAsync(WechatMessage message, CancellationToken cancellationToken = default)
    {
        SaveMessage(message);
        var openId = message.GetValue<string>(WechatMessageKey.FromUserName);
        return HandleMessageAsync(openId, message, cancellationToken);
    }

    protected abstract Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default);

    private async void SaveMessage(WechatMessage message)
    {
        try
        {
            var exists = await _client.From<WechatMessageEntity>()
                                      .Where(t => t.Id == message.MessageId)
                                      .Count(Constants.CountType.Exact)
                                      .ContinueWith(task => task.Result > 0);
            if (exists)
            {
                return;
            }

            var entity = new WechatMessageEntity
            {
                Id = message.MessageId,
                CreateTime = message.CreateTime,
                OpenId = message.GetValue<string>(WechatMessageKey.FromUserName),
                Type = message.MessageType.ToString(),
                Payload = message.GetOriginXml()
            };

            await _client.From<WechatMessageEntity>()
                         .Insert(entity);
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "保存微信消息失败");
        }
    }
}