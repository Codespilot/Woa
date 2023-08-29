namespace Woa.Webapi.Domain;

public class SupabaseRepositoryContext : IRepositoryContext
{
	private readonly IServiceProvider _provider;

	public SupabaseRepositoryContext(IServiceProvider provider, SupabaseClient client)
	{
		_provider = provider;
		Context = client;
	}

	public object Context { get; }

	public IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>()
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>
	{
		return _provider.GetRequiredService<IRepository<TEntity, TKey>>();
	}
}