using System.Linq.Expressions;
using Postgrest;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

public class SupabaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : BaseModel, IEntity<TKey>, new()
    where TKey : IEquatable<TKey>
{
    private readonly SupabaseClient _client;

    public SupabaseRepository(SupabaseClient client)
    {
        _client = client;
    }

    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _client.From<TEntity>()
                            .Where(t => t.Id.Equals(id))
                            .Single(cancellationToken);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _client.From<TEntity>()
                            .Where(predicate)
                            .Single(cancellationToken);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _client.From<TEntity>()
                                    .Insert(entity, cancellationToken: cancellationToken);
        return response.Model;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _client.From<TEntity>()
                                    .Update(entity, cancellationToken: cancellationToken);
        return response.Model;
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _client.From<TEntity>()
                     .Delete(entity, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = new TEntity { Id = id };
        await _client.From<TEntity>()
                     .Delete(entity, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var response = await _client.From<TEntity>()
                                    .Where(predicate)
                                    .Count(Constants.CountType.Planned, cancellationToken);
        return response > 0;
    }

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var response = await _client.From<TEntity>()
                                    .Where(predicate)
                                    .Range(0, 1000)
                                    .Get(cancellationToken);
        return response.Models;
    }

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int offset, int count, CancellationToken cancellationToken = default)
    {
        var response = await _client.From<TEntity>()
                                    .Where(predicate)
                                    .Range(offset, count)
                                    .Get(cancellationToken);
        return response.Models;
    }

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int offset, int count, Expression<Func<TEntity, object>> orderBy, bool ascending, CancellationToken cancellationToken = default)
    {
        var ordering = ascending ? Constants.Ordering.Ascending : Constants.Ordering.Descending;

        var response = await _client.From<TEntity>()
                                    .Where(predicate)
                                    .Range(offset, count)
                                    .Order(orderBy, ordering)
                                    .Get(cancellationToken);
        return response.Models;
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var response = await _client.From<TEntity>()
                                    .Where(predicate)
                                    .Count(Constants.CountType.Planned, cancellationToken);
        return response;
    }
}