using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class WechatFollowerRepository
{
	private readonly IRepository<WechatFollowerEntity, long> _repository;

	public WechatFollowerRepository(IRepository<WechatFollowerEntity, long> repository)
	{
		_repository = repository;
	}

	public Task<WechatFollowerEntity> GetAsync(string openId, CancellationToken cancellationToken = default)
	{
		return GetAsync(t => t.OpenId == openId, cancellationToken);
	}

	public Task<WechatFollowerEntity> GetAsync(string openId, string platformId, CancellationToken cancellationToken = default)
	{
		return GetAsync(t => t.OpenId == openId && t.PlatformId == platformId, cancellationToken);
	}

	public Task<WechatFollowerEntity> GetAsync(Expression<Func<WechatFollowerEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(predicate, cancellationToken);
	}

	public Task InsertAsync(WechatFollowerEntity entity)
	{
		return _repository.InsertAsync(entity);
	}

	public Task UpdateAsync(WechatFollowerEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(entity, cancellationToken);
	}
}