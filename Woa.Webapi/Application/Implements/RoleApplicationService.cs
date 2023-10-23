using System.Linq.Expressions;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class RoleApplicationService : BaseApplicationService, IRoleApplicationService
{
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
        var repository = ServiceProvider.GetRequiredService<IRepository<RoleEntity, int>>();
        return repository.GetAsync(id, cancellationToken)
                         .ContinueWith(task => Mapper.Map<RoleInfoDto>(task.Result), cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<RoleInfoDto>> SearchAsync(RoleQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
    {
        var expressions = new List<Expression<Func<RoleEntity, bool>>>();
        if (!string.IsNullOrWhiteSpace(condition.Keywords))
        {
            expressions.Add(x => x.Code.Contains(condition.Keywords) || x.Name.Contains(condition.Keywords));
        }
        var predicate = expressions.Aggregate(t => t.Id > 0);
        var repository = ServiceProvider.GetRequiredService<IRepository<RoleEntity, int>>();
        return repository.FindAsync(predicate, page, size, cancellationToken)
                         .ContinueWith(task => Mapper.Map<List<RoleInfoDto>>(task.Result), cancellationToken);
    }
}