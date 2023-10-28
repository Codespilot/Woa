using System.Linq.Expressions;

namespace Woa.Webapi.Domain;

public class UserRoleRepository
{
	private readonly IRepository<UserRoleEntity, int> _repository;

	public UserRoleRepository(IRepository<UserRoleEntity, int> repository)
	{
		_repository = repository;
	}

	public Task<List<UserRoleEntity>> GetAsync(int userId, CancellationToken cancellationToken = default)
	{
		return _repository.FindAsync(t => t.UserId == userId, cancellationToken);
	}

	public Task InsertAsync(int userId, int roleId, CancellationToken cancellationToken = default)
	{
		return _repository.InsertAsync(new UserRoleEntity
		{
			UserId = userId,
			RoleId = roleId
		});
	}

	public Task InsertAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken = default)
	{
		var entities = roleIds.Select(roleId => new UserRoleEntity
		{
			UserId = userId,
			RoleId = roleId
		});

		return _repository.InsertAsync(entities, cancellationToken);
	}

	public Task DeleteAsync(int userId, int roleId, CancellationToken cancellationToken = default)
	{
		return DeleteAsync(t => t.UserId == userId && t.RoleId == roleId, cancellationToken);
	}

	public Task DeleteByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
	{
		return DeleteAsync(t => t.RoleId == roleId, cancellationToken);
	}

	public Task DeleteByUserIdAsync(int userId, CancellationToken cancellationToken = default)
	{
		return DeleteAsync(t => t.UserId == userId, cancellationToken);
	}

	public Task DeleteAsync(Expression<Func<UserRoleEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return _repository.DeleteAsync(predicate, cancellationToken);
	}
}