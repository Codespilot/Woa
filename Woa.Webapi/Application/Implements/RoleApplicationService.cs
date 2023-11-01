using System.Linq.Expressions;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class RoleApplicationService : BaseApplicationService, IRoleApplicationService
{
	private RoleRepository _repository;
	private RoleRepository Repository => _repository??= ServiceProvider.GetRequiredService<RoleRepository>();

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
    public Task<RoleInfoDto> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return Repository.GetAsync(id, cancellationToken)
                         .ContinueWith(task => Mapper.Map<RoleInfoDto>(task.Result), cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<RoleInfoDto>> SearchAsync(RoleQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
    {
		var predicate = BuildExpression(condition);

		return Repository.FindAsync(predicate, page, size, cancellationToken)
                         .ContinueWith(task => Mapper.Map<List<RoleInfoDto>>(task.Result), cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> CountAsync(RoleQueryDto condition, CancellationToken cancellationToken = default)
    {
		var predicate = BuildExpression(condition);

		return Repository.CountAsync(predicate, cancellationToken);
    }

	private static Expression<Func<RoleEntity,bool>> BuildExpression(RoleQueryDto condition)
	{
		var expressions = new List<Expression<Func<RoleEntity, bool>>>();
		if (!string.IsNullOrWhiteSpace(condition.Keyword))
		{
			expressions.Add(x => x.Code.Contains(condition.Keyword) || x.Name.Contains(condition.Keyword));
		}
		var predicate = expressions.Aggregate(t => t.Id > 0);

		return predicate;
	}
}