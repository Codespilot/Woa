using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using Woa.Sdk.Wechat;
using Woa.Shared;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 微信用户文本消息处理器
/// </summary>
[WechatMessageHandle(WechatMessageType.Text)]
public class WechatTextMessageHandler : WechatUserMessageHandler
{
	private readonly IRepository<WechatFollowerEntity, long> _repository;
	private readonly WechatOptions _options;

	public WechatTextMessageHandler(IWechatUserMessageStore store, IRepository<WechatFollowerEntity, long> repository, IOptions<WechatOptions> options)
		: base(store)
	{
		_repository = repository;
		_options = options.Value;
	}

	/// <inheritdoc />
	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		var messageContent = message.GetValue<string>(WechatMessageKey.Standard.Content);

		if (string.IsNullOrWhiteSpace(messageContent))
		{
			return WechatMessage.Text("无法识别内容");
		}

		var follower = await _repository.GetAsync(t => t.OpenId == openId, cancellationToken);
		if (follower?.IsChatbotEnabled == true)
		{
			WeakReferenceMessenger.Default.Send(new ChatbotBroadcast { OpenId = openId, MessageId = message.MessageId, MessageContent = messageContent });

			if (_options.EnableCustomMessage)
			{
				// 如果公众号开启了客服消息功能，直接返回null，表示不需要等待聊天机器人回复，因为聊天机器人回复的消息会通过异步方式下发给用户
				return WechatMessage.Empty;
			}

			{
				return new WechatMessage(WechatMessageType.News)
				{
					[WechatMessageKey.Reply.ArticleCount] = 1,
					[WechatMessageKey.Reply.Articles] = new[]
					{
						new
						{
							Title = _options.ReplyTitle,
							Description = _options.ReplyDescription,
							PicUrl = _options.ReplyPicUrl,
							Url = $"{_options.ReplyUrl}{message.MessageId}/reply"
						}
					}
				};
			}
		}

		{
		}

		return WechatMessage.Text("您已关闭聊天机器人功能，无法使用此功能。");
	}
}