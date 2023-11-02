using System.Linq.Expressions;
using Woa.Common;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class RoleApplicationService : BaseApplicationService, IRoleApplicationService
{
	private RoleRepository _repository;

	private RoleRepository Repository => _repository ??= ServiceProvider.GetRequiredService<RoleRepository>();

	/// <inheritdoc />
	public Task<int> CreateAsync(RoleEditDto dto, CancellationToken cancellationToken = default)
	{
		var command = Mapper.Map<RoleCreateCommand>(dto);
		return Mediator.Send(command, cancellationToken);
	}

	/// <inheritdoc />
	public Task UpdateAsync(int id, RoleEditDto dto, CancellationToken cancellationToken = default)
	{
		var command = new RoleUpdateCommand(id);
		Mapper.Map(dto, command);
		return Mediator.Send(command, cancellationToken);
	}

	/// <inheritdoc />
	public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var command = new RoleDeleteCommand(id);
		return Mediator.Send(command, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<RoleInfoDto> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		var entity = await Repository.GetAsync(id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException("角色不存在");
		}

		var users = await ServiceProvider.GetService<CommonRepository>().GetUserNameAsync(new[] { entity.CreateBy, entity.UpdateBy ?? 0 }, t => t.Username, cancellationToken);

		return Mapper.Map<RoleInfoDto>(entity, opts => opts.Items["users"] = users);
	}

	/// <inheritdoc />
	public async Task<List<RoleInfoDto>> SearchAsync(RoleQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		var entities = await Repository.FindAsync(predicate, page, size, cancellationToken);

		var userIds = entities.SelectMany(t => new[] { t.CreateBy, t.UpdateBy ?? 0 })
							  .Where(t => t > 0)
							  .Distinct()
							  .ToList();

		var users = await ServiceProvider.GetService<CommonRepository>().GetUserNameAsync(userIds, t => t.Username, cancellationToken);

		var items = Mapper.Map<List<RoleInfoDto>>(entities, opts => opts.Items["users"] = users);

		return items;
	}

	/// <inheritdoc />
	public Task<int> CountAsync(RoleQueryDto condition, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		return Repository.CountAsync(predicate, cancellationToken);
	}

	#region Supports
	private static Expression<Func<RoleEntity, bool>> BuildExpression(RoleQueryDto condition)
	{
		var expressions = new List<Expression<Func<RoleEntity, bool>>>();
		if (!string.IsNullOrWhiteSpace(condition.Keyword))
		{
			expressions.Add(x => x.Code.Contains(condition.Keyword) || x.Name.Contains(condition.Keyword));
		}
		var predicate = expressions.Aggregate(t => t.Id > 0);

		return predicate;
	}
	#endregion
}