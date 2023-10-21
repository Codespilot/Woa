using MediatR;
using System.Linq.Expressions;
using Woa.Common;

namespace Woa.Webapi.Domain;

public sealed class RoleCommandHandler : ICommandHandler<RoleCreateCommand, int>,
                                         ICommandHandler<RoleUpdateCommand>,
                                         ICommandHandler<RoleDeleteCommand>
{
    private readonly IRepository<RoleEntity, int> _repository;

    public RoleCommandHandler(IRepository<RoleEntity, int> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
    {
        {
            var exists = await CheckExistsAsync(t => t.Code == request.Code);
            if (exists)
            {
                throw new BadRequestException("角色代码已经存在");
            }
        }

        var entity = new RoleEntity
        {
            Code = request.Code,
            Name = request.Name
        };

        var result = await _repository.InsertAsync(entity, cancellationToken);
        return result.Id;
    }

    public async Task Handle(RoleUpdateCommand request, CancellationToken cancellationToken)
    {
        {
            var exists = await CheckExistsAsync(t => t.Code == request.Code && t.Id != request.Id);
            if (exists)
            {
                throw new BadRequestException("角色代码已经存在");
            }
        }

        var entity = await _repository.GetAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException("角色不存在");
        }

        entity.Code = request.Code;
        entity.Name = request.Name;

        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public async Task Handle(RoleDeleteCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.Id, cancellationToken);
    }

    private async Task<bool> CheckExistsAsync(Expression<Func<RoleEntity, bool>> predicate)
    {
        return await _repository.ExistsAsync(predicate);
    }
}