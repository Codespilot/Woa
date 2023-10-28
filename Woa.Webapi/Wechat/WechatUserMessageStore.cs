using Woa.Common;
using Woa.Sdk.Tencent;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Wechat;

public class WechatUserMessageStore : IWechatUserMessageStore
{
	private readonly WechatMessageRepository _repository;
	private readonly ILogger<WechatUserMessageStore> _logger;

	public WechatUserMessageStore(WechatMessageRepository repository, ILoggerFactory logger)
	{
		_repository = repository;
		_logger = logger.CreateLogger<WechatUserMessageStore>();
	}

	public async Task SaveAsync(WechatMessage message, CancellationToken cancellationToken = default)
	{
		try
		{
			var exists = await _repository.ExistsAsync(t => t.Id == message.MessageId, cancellationToken);
			if (exists)
			{
				return;
			}

			var entity = new WechatMessageEntity
			{
				Id = message.MessageId,
				CreateTime = message.CreateTime,
				OpenId = message.GetValue<string>(WechatMessageKey.FromUserName),
				PlatformId = message.GetValue<string>(WechatMessageKey.ToUserName),
				Type = message.MessageType.ToString(),
				Payload = Cryptography.Base64.Encrypt(message.GetOriginXml())
			};

			switch (message.MessageType)
			{
				case WechatMessageType.Text:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.Content);
					break;
				case WechatMessageType.Image:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.PictureUrl)?.Replace("http://", "https://");
					break;
				case WechatMessageType.Voice:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.Recognition);
					break;
				case WechatMessageType.Video:
				case WechatMessageType.ShortVideo:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.MediaId);
					break;
				case WechatMessageType.Location:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.Label);
					break;
				case WechatMessageType.Link:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.Title);
					break;
				case WechatMessageType.Event:
					entity.Content = message.GetValue<string>(WechatMessageKey.Event.EventType) switch
					{
						"subscribe" => "用户关注了公众号",
						"unsubscribe" => "用户取消关注了公众号",
						"scan" => "用户扫描了二维码",
						"location" => "用户上报了地理位置",
						"click" => "用户点击了菜单",
						"view" => "用户点击了菜单",
						_ => "未知事件"
					};
					break;
			}

			await _repository.InsertAsync(entity, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "保存微信消息失败");
		}
	}
}