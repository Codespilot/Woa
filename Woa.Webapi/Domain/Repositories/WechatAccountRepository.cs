using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class WechatAccountRepository
{
	private readonly IRepository<WechatAccountEntity, string> _repository;

	public WechatAccountRepository(IRepository<WechatAccountEntity, string> repository)
	{
		_repository = repository;
	}

	public Task<WechatAccountEntity> GetAsync(string id, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(id, cancellationToken);
	}

	public Task<WechatAccountEntity> GetAsync(Expression<Func<WechatAccountEntity, bool>> condition, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(condition, cancellationToken);
	}

	public Task<List<WechatAccountEntity>> FindAsync(Expression<Func<WechatAccountEntity, bool>> condition, int page, int size, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(condition, page, size, t => t.CreateAt, false, cancellationToken);
	}

	public Task<int> CountAsync(Expression<Func<WechatAccountEntity, bool>> condition, CancellationToken cancellationToken = default)
	{
		return _repository.CountAsync(condition, cancellationToken);
	}

	public Task<WechatAccountEntity> InsertAsync(WechatAccountEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.InsertAsync(entity, cancellationToken);
	}

	public Task UpdateAsync(WechatAccountEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(entity, cancellationToken);
	}
	
	public Task UpdateAsync(string id, Action<WechatAccountEntity> action, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(id, action, cancellationToken);
	}
}