using Woa.Common;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class WechatMessageApplicationService : BaseApplicationService, IWechatMessageApplicationService
{
	private readonly IRepository<WechatMessageEntity, long> _repository;

	public WechatMessageApplicationService(IRepository<WechatMessageEntity, long> repository)
	{
		_repository = repository;
	}

	public async Task<List<WechatMessageItemDto>> SearchAsync(WechatMenuQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	public async Task<WechatMessageDetailDto> GetAsync(long id, CancellationToken cancellationToken = default)
	{
		var entity = await _repository.GetAsync(id, cancellationToken);
		return default;
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
}