namespace Woa.Webapi;

public interface IRepositoryContext
{
	object Context { get; }

	IRepository<TEntity, TKey> CreateRepository<TEntity, TKey>()
		where TEntity : class, IEntity<TKey>
		where TKey : IEquatable<TKey>;
}