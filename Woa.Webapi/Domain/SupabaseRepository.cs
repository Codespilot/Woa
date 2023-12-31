﻿using System.Linq.Expressions;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using Polly;
using Postgrest.Attributes;
using Postgrest.Models;
using Woa.Common;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Woa.Webapi.Domain;

public class SupabaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
	where TEntity : BaseModel, IEntity<TKey>, new()
	where TKey : IEquatable<TKey>
{
	private readonly SupabaseClient _client;
	private readonly ILogger<SupabaseRepository<TEntity, TKey>> _logger;
	private readonly IIdentityContext _identity;

	public SupabaseRepository(SupabaseClient client, ILoggerFactory logger, IIdentityContext identity)
	{
		_client = client;
		_logger = logger.CreateLogger<SupabaseRepository<TEntity, TKey>>();
		_identity = identity;
	}

	public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
	{
		var predicate = ExpressionHelper.BuildPropertyEqualsExpression<TEntity, TKey>(id, "Id");
		return await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Single(cancellationToken)
		);
	}

	public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Single(cancellationToken)
		);
	}

	public Task<List<TEntity>> GetAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
	{
		var property = typeof(TEntity).GetProperty("Id");

		if (property == null)
		{
			throw new InternalServerException("实体没有Id属性");
		}

		var columnName = GetColumnName(property);

		var criterion = ids.Select(t => t as object).ToList();
		return ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Filter(columnName, PostgrestConstants.Operator.In, criterion)
				   .Get(cancellationToken)
				   .ContinueWith(task => task.Result.Models, cancellationToken)
		);
	}

	public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		if (entity is ICreateAudit audit)
		{
			audit.CreateBy = _identity.Id;
			audit.CreateAt = DateTime.UtcNow;
		}

		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Insert(entity, cancellationToken: cancellationToken)
		);
		return response.Model;
	}

	public async Task InsertAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
	{
		foreach (var entity in entities)
		{
			if (entity is not ICreateAudit audit)
			{
				continue;
			}

			audit.CreateBy = _identity.Id;
			audit.CreateAt = DateTime.UtcNow;
		}

		await ExecuteAsync(async () =>
		{
			await _client.From<TEntity>()
						 .Insert(entities, cancellationToken: cancellationToken);
		});
	}

	public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		if (entity is IUpdateAudit audit)
		{
			audit.UpdateBy = _identity.Id;
			audit.UpdateAt = DateTime.UtcNow;
		}

		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Update(entity, cancellationToken: cancellationToken)
		);
		return response.Model;
	}

	public async Task<TEntity> UpdateAsync(TKey id, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
	{
		var entity = await GetAsync(id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException();
		}
		updateAction(entity);
		if (entity is IUpdateAudit audit)
		{
			audit.UpdateBy = _identity.Id;
			audit.UpdateAt = DateTime.UtcNow;
		}

		{
		}
		return await UpdateAsync(entity, cancellationToken);
	}

	public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
	{
		await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Delete(entity, cancellationToken: cancellationToken)
		);
	}

	public Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
	{
		var predicate = ExpressionHelper.BuildPropertyEqualsExpression<TEntity, TKey>(id, "Id");
		return DeleteAsync(predicate, cancellationToken);
	}

	public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Delete(cancellationToken: cancellationToken)
		);
	}

	public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Count(PostgrestConstants.CountType.Exact, cancellationToken)
		);
		return response > 0;
	}

	public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Get(cancellationToken)
		);
		return response.Models;
	}

	public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int count, CancellationToken cancellationToken = default)
	{
		var offset = GetOffset(page, count);

		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Range(offset, offset + count - 1)
				   .Get(cancellationToken)
		);
		return response.Models;
	}

	public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int count, Expression<Func<TEntity, object>> orderBy, bool ascending, CancellationToken cancellationToken = default)
	{
		var offset = GetOffset(page, count);

		var ordering = ascending ? PostgrestConstants.Ordering.Ascending : PostgrestConstants.Ordering.Descending;

		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Range(offset, page + count - 1)
				   .Order(orderBy, ordering)
				   .Get(cancellationToken)
		);
		return response.Models;
	}

	public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, object>> predicate, string @operator, object criterion, CancellationToken cancellationToken = default)
	{
		var response = await ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Filter(predicate, GetOperator(@operator), criterion: criterion)
				   .Get(cancellationToken)
		);
		return response.Models;
	}

	public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
	{
		return ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Where(predicate)
				   .Count(PostgrestConstants.CountType.Exact, cancellationToken)
		);
	}

	public Task<int> CountAsync(Expression<Func<TEntity, object>> predicate, string @operator, object criterion, CancellationToken cancellationToken = default)
	{
		return ExecuteAsync(() =>
			_client.From<TEntity>()
				   .Filter(predicate, GetOperator(@operator), criterion)
				   .Count(PostgrestConstants.CountType.Exact, cancellationToken)
		);
	}

	private Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> handle)
	{
		return Policy.Handle<OperationCanceledException>()
					 .Or<HttpRequestException>()
					 .Or<WebSocketException>()
					 .Or<WebException>()
					 .Or<TimeoutException>()
					 .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
					 .ExecuteAsync(handle);
	}

	private Task ExecuteAsync(Func<Task> handle)
	{
		return Policy.Handle<OperationCanceledException>()
					 .Or<HttpRequestException>()
					 .Or<WebSocketException>()
					 .Or<WebException>()
					 .Or<TimeoutException>()
					 .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
					 .ExecuteAsync(handle);
	}

	private void OnRetry(Exception exception, TimeSpan timeSpan, int retryCount, object context)
	{
		_logger.LogError(exception, "第{RetryCount}次重试，等待{TimeSpan}后重试", retryCount, timeSpan);
	}

	private static int GetOffset(int page, int count)
	{
		return (page - 1) * count;
	}

	private static PostgrestConstants.Operator GetOperator(string @operator)
	{
		@operator = @operator.ToLowerInvariant();

		return @operator switch
		{
			"and" => PostgrestConstants.Operator.And,
			"or" => PostgrestConstants.Operator.Or,
			"eq" => PostgrestConstants.Operator.Equals,
			"gt" => PostgrestConstants.Operator.GreaterThan,
			"gte" => PostgrestConstants.Operator.GreaterThanOrEqual,
			"lt" => PostgrestConstants.Operator.LessThan,
			"lte" => PostgrestConstants.Operator.LessThanOrEqual,
			"neq" => PostgrestConstants.Operator.NotEqual,
			"like" => PostgrestConstants.Operator.Like,
			"ilike" => PostgrestConstants.Operator.ILike,
			"in" => PostgrestConstants.Operator.In,
			"is" => PostgrestConstants.Operator.Is,
			"fts" => PostgrestConstants.Operator.FTS,
			"plfts" => PostgrestConstants.Operator.PLFTS,
			"phfts" => PostgrestConstants.Operator.PHFTS,
			"wfts" => PostgrestConstants.Operator.WFTS,
			"cs" => PostgrestConstants.Operator.Contains,
			"cd" => PostgrestConstants.Operator.ContainedIn,
			"ov" => PostgrestConstants.Operator.Overlap,
			"sl" => PostgrestConstants.Operator.StrictlyLeft,
			"sr" => PostgrestConstants.Operator.StrictlyRight,
			"nxr" => PostgrestConstants.Operator.NotRightOf,
			"nxl" => PostgrestConstants.Operator.NotLeftOf,
			"adj" => PostgrestConstants.Operator.Adjacent,
			"not" => PostgrestConstants.Operator.Not,
			_ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null)
		};
	}

	private static string GetColumnName(PropertyInfo property)
	{
		var delegations = new List<Func<PropertyInfo, string>>
		{
			p => p.GetCustomAttribute<ColumnAttribute>()?.ColumnName,
			p => p.GetCustomAttribute<PrimaryKeyAttribute>()?.ColumnName,
		};

		foreach (var func in delegations)
		{
			var column = func(property);
			if (!string.IsNullOrWhiteSpace(column))
			{
				return column;
			}
		}

		throw new InternalServerException("实体没有配置有效的字段映射");
	}
}