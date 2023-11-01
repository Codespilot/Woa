using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class UserRepository
{
	private readonly IRepository<UserEntity, int> _repository;

	public UserRepository(IRepository<UserEntity, int> repository)
	{
		_repository = repository;
	}

	public Task<UserEntity> GetAsync(string username, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(t => t.Username == username, cancellationToken);
	}

	public Task<UserEntity> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(id, cancellationToken);
	}

	public Task<List<UserEntity>> GetAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
	{
		return _repository.GetAsync(ids, cancellationToken);
	}

	public Task<List<UserEntity>> FindAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(predicate, cancellationToken);
	}

	public Task<List<UserEntity>> FindAsync(Expression<Func<UserEntity, bool>> predicate, int page, int size, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(predicate, page, size, entity => entity.Id, false, cancellationToken);
	}

	public Task<int> CountAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.CountAsync(predicate, cancellationToken);
	}

	public Task<UserEntity> InsertAsync(UserEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.InsertAsync(entity, cancellationToken);
	}

	public Task UpdateAsync(UserEntity entity, CancellationToken cancellationToken = default)
	{
		return _repository.UpdateAsync(entity, cancellationToken);
	}

	public Task<bool> ExistsAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.ExistsAsync(predicate, cancellationToken);
	}
}
