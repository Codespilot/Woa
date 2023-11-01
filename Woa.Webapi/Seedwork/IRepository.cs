using System.Linq.Expressions;

namespace Woa.Webapi;

public interface IRepository<TEntity, in TKey>
	where TEntity : class, IEntity<TKey>
	where TKey : IEquatable<TKey>
{
	Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);

	Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

	Task<List<TEntity>> GetAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

	Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

	Task InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

	Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

	Task<TEntity> UpdateAsync(TKey id, Action<TEntity> updateAction, CancellationToken cancellationToken = default);

	Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

	Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);

	Task DeleteAsync(Expression<Func<TEntity,bool>> predicate, CancellationToken cancellationToken = default);

	Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

	Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

	Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int count, CancellationToken cancellationToken = default);

	Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int count, Expression<Func<TEntity, object>> orderBy, bool ascending, CancellationToken cancellationToken = default);

	Task<List<TEntity>> FindAsync(Expression<Func<TEntity, object>> predicate, string @operator, object criterion, CancellationToken cancellationToken = default);

	Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

	Task<int> CountAsync(Expression<Func<TEntity, object>> predicate, string @operator, object criterion, CancellationToken cancellationToken = default);
}