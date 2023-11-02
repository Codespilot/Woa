namespace Woa.Webapi.Domain;

public class CommonRepository
{
	private readonly IServiceProvider _provider;

	public CommonRepository(IServiceProvider provider)
	{
		_provider = provider;
	}

	public async Task<Dictionary<TKey, string>> GetNamesAsync<TEntity, TKey>(IEnumerable<TKey> ids, Func<TEntity, string> selector, CancellationToken cancellationToken = default)
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		Dictionary<TKey, string> result;

		if (ids?.All(id => id == null) == true)
		{
			var repository = _provider.GetService<IRepository<TEntity, TKey>>();

			var entities = await repository.GetAsync(ids, cancellationToken);
			result = entities.ToDictionary(t => t.Id, selector);
		}
		else
		{
			result = new Dictionary<TKey, string>();
		}
		return result;
	}

	public Task<Dictionary<int, string>> GetUserNameAsync(IEnumerable<int> ids, Func<UserEntity, string> selector, CancellationToken cancellationToken = default)
	{
		return GetNamesAsync(ids, selector, cancellationToken);
	}

	public Task<Dictionary<int, string>> GetRoleNameAsync(IEnumerable<int> ids, Func<RoleEntity, string> selector, CancellationToken cancellationToken = default)
	{
		return GetNamesAsync(ids, selector, cancellationToken);
	}

	public Task<Dictionary<string, string>> GetWechatAccountNameAsync(IEnumerable<string> ids, Func<WechatAccountEntity, string> selector, CancellationToken cancellationToken = default)
	{
		return GetNamesAsync(ids, selector, cancellationToken);
	}
}
