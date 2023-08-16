using Postgrest;
using Woa.Sdk.Wechat;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Services;

public class WechatUserMessageStore : IWechatUserMessageStore
{
	private readonly SupabaseClient _client;
	private readonly ILogger<WechatUserMessageStore> _logger;

	public WechatUserMessageStore(SupabaseClient client, ILoggerFactory logger)
	{
		_client = client;
		_logger = logger.CreateLogger<WechatUserMessageStore>();
	}

	public async Task SaveAsync(WechatMessage message, CancellationToken cancellationToken = default)
	{
		try
		{
			var exists = await _client.From<WechatMessageEntity>()
			                          .Where(t => t.Id == message.MessageId)
			                          .Count(Constants.CountType.Exact, cancellationToken)
			                          .ContinueWith(task => task.Result > 0, cancellationToken);
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
			             .Insert(entity, cancellationToken: cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "保存微信消息失败");
		}
	}
}