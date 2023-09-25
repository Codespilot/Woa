using Woa.Sdk.Wechat;
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
				Payload = message.GetOriginXml()
			};

			await _repository.InsertAsync(entity, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "保存微信消息失败");
		}
	}
}