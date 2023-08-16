using CommunityToolkit.Mvvm.Messaging;
using Woa.Sdk.Wechat;
using Woa.Webapi.Domain;
using Woa.Webapi.Services;

namespace Woa.Webapi.Handlers;

[WechatMessageHandle(WechatMessageType.Voice)]
public class WechatVoiceMessageHandler : WechatUserMessageHandler
{
	private readonly SupabaseClient _client;
	private readonly IConfiguration _configuration;

	public WechatVoiceMessageHandler(IWechatUserMessageStore store, SupabaseClient client, IConfiguration configuration)
		: base(store)
	{
		_client = client;
		_configuration = configuration;
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default)
    {
        var messageContent = message.GetValue<string>(WechatMessageKey.Standard.Recognition);

        if (string.IsNullOrWhiteSpace(messageContent))
        {
            return new WechatMessage(WechatMessageType.Text)
            {
                [WechatMessageKey.Reply.Content] = "无法识别内容"
            };
        }

        var user = await _client.From<WechatFollowerEntity>()
                                       .Where(t => t.OpenId == openId)
                                       .Single(cancellationToken);

        if (user?.IsChatbotEnabled == true)
        {
            WeakReferenceMessenger.Default.Send(new ChatbotBroadcast { OpenId = openId, MessageId = message.MessageId, MessageContent = messageContent });

            if (_configuration.GetValue<bool>("Wechat:EnableCustomMessage"))
            {
                // 如果公众号开启了客服消息功能，直接返回null，表示不需要等待聊天机器人回复，因为聊天机器人回复的消息会通过异步方式下发给用户
                return WechatMessage.Empty;
            }

            {
                var replyUrl = _configuration.GetValue<string>("Wechat:ReplyUrl");
                return new WechatMessage(WechatMessageType.News)
                {
                    [WechatMessageKey.Reply.ArticleCount] = 1,
                    [WechatMessageKey.Reply.Articles] = new[]
                    {
                        new
                        {
                            Title = _configuration.GetValue<string>("Wechat:ReplyTitle"),
                            Description = _configuration.GetValue<string>("Wechat:ReplyDescription"),
                            PicUrl = _configuration.GetValue<string>("Wechat:ReplyPicUrl"),
                            Url = $"{replyUrl}{message.MessageId}/reply"
                        }
                    }
                };
            }
        }

        {
        }

        return new WechatMessage(WechatMessageType.Text)
        {
            [WechatMessageKey.Reply.Content] = "您已关闭聊天机器人功能，无法使用此功能。"
        };
    }
}