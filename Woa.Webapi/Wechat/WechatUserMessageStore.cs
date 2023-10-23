using Woa.Common;
using Woa.Sdk.Tencent;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Wechat;

public class WechatUserMessageStore : IWechatUserMessageStore
{
	private readonly IRepository<WechatMessageEntity, long> _repository;
	private readonly ILogger<WechatUserMessageStore> _logger;

	public WechatUserMessageStore(IRepository<WechatMessageEntity, long> repository, ILoggerFactory logger)
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
				AccountId = message.GetValue<string>(WechatMessageKey.ToUserName),
				Type = message.MessageType.ToString(),
				Payload = Cryptography.Base64.Encrypt(message.GetOriginXml())
			};

			switch (message.MessageType)
			{
				case WechatMessageType.Text:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.Content);
					break;
				case WechatMessageType.Image:
					entity.Content = message.GetValue<string>(WechatMessageKey.Standard.PictureUrl);
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
			}

			await _repository.InsertAsync(entity, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "保存微信消息失败");
		}
	}
}