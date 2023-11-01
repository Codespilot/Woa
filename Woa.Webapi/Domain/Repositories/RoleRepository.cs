using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class RoleRepository
{
	private readonly IRepository<RoleEntity, int> _repository;

	public RoleRepository(IRepository<RoleEntity, int> repository)
	{
		_repository = repository;
	}

	public Task<RoleEntity> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(id, cancellationToken);
	}

	public Task<List<RoleEntity>> GetAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
	{
		var roleIds = ids.Select(t => t as object).ToList();

		// Supabase client不支持 xxx.Contains(t.Id)表达式，所以这里改成了下面的写法
		return _repository.FindAsync(t => t.Id, QueryOperator.In, roleIds, cancellationToken);
	}

	public Task<List<RoleEntity>> FindAsync(Expression<Func<RoleEntity, bool>> predicate, int page, int size, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(predicate, page, size, t => t.Id, false, cancellationToken);
	}

	public Task<int> CountAsync(Expression<Func<RoleEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.CountAsync(predicate, cancellationToken);
	}

	public Task<RoleEntity> InsertAsync(RoleEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.InsertAsync(entity, cancellationToken);
	}

	public Task UpdateAsync(RoleEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(entity, cancellationToken);
	}

	public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		return _repository.DeleteAsync(id, cancellationToken);
	}

	public Task<bool> ExistsAsync(Expression<Func<RoleEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.ExistsAsync(predicate, cancellationToken);
	}

	public Task<bool> ExistsAsync(string code, int originId, CancellationToken cancellationToken = default)
	{
		return ExistsAsync(t => t.Code == code && t.Id != originId, cancellationToken);
	}
}