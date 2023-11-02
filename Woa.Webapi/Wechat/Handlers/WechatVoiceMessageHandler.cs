using CommunityToolkit.Mvvm.Messaging;
using Woa.Sdk.Tencent;
using Woa.Shared;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 微信用户语音消息处理器
/// </summary>
[WechatMessageHandle(WechatMessageType.Voice)]
public class WechatVoiceMessageHandler : WechatUserMessageHandler
{
	private readonly WechatFollowerRepository _repository;

	public WechatVoiceMessageHandler(IWechatUserMessageStore store, WechatFollowerRepository repository, WechatOptions options)
		: base(store, options)
	{
		_repository = repository;
	}

	/// <inheritdoc />
	protected override async Task<WechatMessage> HandleMessageAsync(string openId, string platformId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		var messageContent = message.GetValue<string>(WechatMessageKey.Standard.Recognition);

		if (string.IsNullOrWhiteSpace(messageContent))
		{
			return new WechatMessage(WechatMessageType.Text)
			{
				[WechatMessageKey.Reply.Content] = "无法识别内容"
			};
		}

		var follower = await _repository.GetAsync(openId, platformId, cancellationToken);

		if (follower?.IsChatbotEnabled == true)
		{
			WeakReferenceMessenger.Default.Send(new ChatbotBroadcast { OpenId = openId, MessageId = message.MessageId, MessageContent = messageContent });

			if (Options.Accounts[platformId].EnableCustomMessage)
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
							Title = Options.ReplyTitle,
							Description = Options.ReplyDescription,
							PicUrl = Options.ReplyPicUrl,
							Url = $"{Options.ReplyUrl}{message.MessageId}/reply"
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