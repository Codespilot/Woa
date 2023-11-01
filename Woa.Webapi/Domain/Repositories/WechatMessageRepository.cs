using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class WechatMessageRepository
{
	private readonly IRepository<WechatMessageEntity, long> _repository;

	public WechatMessageRepository(IRepository<WechatMessageEntity, long> repository)
	{
		_repository = repository;
	}

	public Task<WechatMessageEntity> GetAsync(long id, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(id, cancellationToken);
	}

	public Task<WechatMessageEntity> GetAsync(Expression<Func<WechatMessageEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(predicate, cancellationToken);
	}

	public Task<List<WechatMessageEntity>> FindAsync(Expression<Func<WechatMessageEntity, bool>> predicate, int page, int size, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(predicate, page, size, t => t.CreateTime, false, cancellationToken);
	}

	public Task<int> CountAsync(Expression<Func<WechatMessageEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.CountAsync(predicate, cancellationToken);
	}

	public Task<bool> ExistsAsync(Expression<Func<WechatMessageEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.ExistsAsync(predicate, cancellationToken);
	}

	public Task<WechatMessageEntity> InsertAsync(WechatMessageEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.InsertAsync(entity, cancellationToken);
	}

	public Task UpdateAsync(WechatMessageEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(entity, cancellationToken);
	}

	public Task UpdateAsync(long id, Action<WechatMessageEntity> action, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(id, action, cancellationToken);
	}

	public Task DeleteAsync(long id, CancellationToken cancellationToken = default)
	{
		return _repository.DeleteAsync(id, cancellationToken);
	}
}
