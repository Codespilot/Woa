using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Woa.Common;
using Woa.Sdk.Wechat;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class WechatMessageApplicationService : BaseApplicationService, IWechatMessageApplicationService
{
	private readonly IRepository<WechatMessageEntity, long> _repository;
	private readonly IWechatApi _api;

	public WechatMessageApplicationService(IRepository<WechatMessageEntity, long> repository, IWechatApi api)
	{
		_repository = repository;
		_api = api;
	}

	public async Task<List<WechatMessageItemDto>> SearchAsync(WechatMessageQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var offset = (page - 1) * size;

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

		var entities = await _repository.FindAsync(predicate, offset, size, cancellationToken);
		var result = Mapper.Map<List<WechatMessageItemDto>>(entities);
		return result;
	}

	public async Task<WechatMessageDetailDto> GetAsync(long id, CancellationToken cancellationToken = default)
	{
		var entity = await _repository.GetAsync(id, cancellationToken);

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
			var response = await _api.GetTemporaryMediaAsync(token, mediaId, cancellationToken);
			return response?.Content?.TryGetValue(name, out var url) == true ? url : response.RequestMessage.RequestUri.OriginalString;
		}
	}

	public async Task<string> GetReplyAsync(long id, CancellationToken cancellationToken = default)
	{
		var entity = await _repository.GetAsync(id, cancellationToken);
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
}