using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Woa.Common;
using Woa.Sdk.Tencent;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class WechatMessageApplicationService : BaseApplicationService, IWechatMessageApplicationService
{
	private WechatMessageRepository _repository;
	private IWechatApi _wechatApi;

	private WechatMessageRepository Repository => _repository ??= ServiceProvider.GetService<WechatMessageRepository>();
	private IWechatApi WechatApi => _wechatApi ??= ServiceProvider.GetService<IWechatApi>();

	public async Task<List<WechatMessageItemDto>> SearchAsync(WechatMessageQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		var entities = await Repository.FindAsync(predicate, page, size, cancellationToken);
		var result = Mapper.Map<List<WechatMessageItemDto>>(entities);
		return result;
	}

	public async Task<int> CountAsync(WechatMessageQueryDto condition, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		var count = await Repository.CountAsync(predicate, cancellationToken);

		return count;
	}

	public async Task<WechatMessageDetailDto> GetAsync(long id, CancellationToken cancellationToken = default)
	{
		var entity = await Repository.GetAsync(id, cancellationToken);

		if (entity == null)
		{
			throw new NotFoundException("微信消息不存在");
		}

		var result = Mapper.Map<WechatMessageDetailDto>(entity);

		var payload = Cryptography.Base64.Decrypt(entity.Payload);

		var message = WechatMessage.Parse(payload);
		var type = Enum.Parse<WechatMessageType>(entity.Type, true);

		switch (type)
		{
			case WechatMessageType.Text:
				{
					result.Detail = message.GetValue<string>(WechatMessageKey.Standard.Content);
				}
				break;
			case WechatMessageType.Image:
				{
					result.Detail = message.GetValue<string>(WechatMessageKey.Standard.PictureUrl);
				}
				break;
			case WechatMessageType.Voice:
				{
					var mediaId = message.GetValue<string>(WechatMessageKey.Standard.MediaId);
					var url = await GetMediaUrlAsync(mediaId, "voice_url");
					result.Detail = new
					{
						Url = url,
						Recognition = message.GetValue<string>(WechatMessageKey.Standard.Recognition)
					};
				}
				break;
			case WechatMessageType.Video:
			case WechatMessageType.ShortVideo:
				{
					var mediaId = message.GetValue<string>(WechatMessageKey.Standard.MediaId);
					result.Detail = await GetMediaUrlAsync(mediaId, "video_url");
				}
				break;
			case WechatMessageType.Location:
				{
					var longitude = message.GetValue<string>(WechatMessageKey.Standard.Longitude);
					var latitude = message.GetValue<string>(WechatMessageKey.Standard.Latitude);
					result.Detail = $"{longitude},{latitude}";
				}
				break;
			case WechatMessageType.Link:
				{
					result.Detail = new
					{
						Title = message.GetValue<string>(WechatMessageKey.Standard.Title),
						Description = message.GetValue<string>(WechatMessageKey.Standard.Description),
						Url = message.GetValue<string>(WechatMessageKey.Standard.Url)
					};
				}
				break;
		}

		return result;

		async Task<string> GetMediaUrlAsync(string mediaId, string name)
		{
			var token = Cache.Get<string>(Constants.Cache.WechatAccessToken);
			var response = await WechatApi.GetTemporaryMediaAsync(token, mediaId, cancellationToken);
			return response?.Content?.TryGetValue(name, out var url) == true ? url : response.RequestMessage.RequestUri.OriginalString;
		}
	}

	public async Task<string> GetReplyAsync(long id, CancellationToken cancellationToken = default)
	{
		var entity = await Repository.GetAsync(id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException("微信消息不存在");
		}

		return entity.Reply;
	}

	public async Task ReplyAsync(long id, string content, CancellationToken cancellationToken = default)
	{
		var command = new WechatMessageReplyCommand(id, content);
		await Mediator.Send(command, cancellationToken);
	}

	public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
	{
		var command = new WechatMessageDeleteCommand(id);
		await Mediator.Send(command, cancellationToken);
	}

	private static Expression<Func<WechatMessageEntity, bool>> BuildExpression(WechatMessageQueryDto condition)
	{
		var expressions = new List<Expression<Func<WechatMessageEntity, bool>>>();

		if (!string.IsNullOrEmpty(condition.Type))
		{
			expressions.Add(t => t.Type == condition.Type);
		}

		if (!string.IsNullOrWhiteSpace(condition.OpenId))
		{
			expressions.Add(t => t.OpenId == condition.OpenId);
		}

		var predicate = expressions.Aggregate(t => t.Id > 0);

		return predicate;
	}
}