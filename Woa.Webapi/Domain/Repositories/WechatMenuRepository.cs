using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class WechatMenuRepository
{
	private readonly IRepository<WechatMenuEntity, int> _repository;

	public WechatMenuRepository(IRepository<WechatMenuEntity, int> repository)
	{
		_repository = repository;
	}

	public Task<List<WechatMenuEntity>> FindAsync(string platformId, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(t => t.PlatformId == platformId, cancellationToken);
	}

	public Task<WechatMenuEntity> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(id, cancellationToken);
	}

	public Task<WechatMenuEntity> GetAsync(Expression<Func<WechatMenuEntity,bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(predicate, cancellationToken);
	}

	public Task InsertAsync(WechatMenuEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.InsertAsync(entity, cancellationToken);
	}

	public Task UpdateAsync(WechatMenuEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(entity, cancellationToken);
	}

	public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		return _repository.DeleteAsync(id, cancellationToken);
	}

	public Task<List<WechatMenuEntity>> FindAsync(Expression<Func<WechatMenuEntity, bool>> predicate, int page, int size, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(predicate, page, size, entity => entity.Id, false, cancellationToken);
	}

	public Task<List<WechatMenuEntity>> FindAsync(Expression<Func<WechatMenuEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(predicate, cancellationToken);
	}

	public Task<int> CountAsync(Expression<Func<WechatMenuEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.CountAsync(predicate, cancellationToken);
	}

	public async Task<int> GetMaximumSortAsync(int parentId)
	{
		var entities = await _repository.FindAsync(t => t.ParentId == parentId, 0, 1, t => t.Sort, false);
		if (entities.Count == 0)
		{
			return 0;
		}

		return entities.Max(t => t.Sort);
	}
}