using MediatR;
using Woa.Common;

namespace Woa.Webapi.Domain;

public sealed class RoleCommandHandler : ICommandHandler<RoleCreateCommand, int>,
										 ICommandHandler<RoleUpdateCommand>,
										 ICommandHandler<RoleDeleteCommand>
{
	private readonly RoleRepository _repository;
	private readonly IMediator _mediator;

	public RoleCommandHandler(RoleRepository repository, IMediator mediator)
	{
		_repository = repository;
		_mediator = mediator;
	}

	public async Task<int> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
	{
		var exists = await _repository.ExistsAsync(request.Code, 0, cancellationToken);
		if (exists)
		{
			throw new BadRequestException("角色代码已经存在");
		}

		var entity = new RoleEntity
		{
			Code = request.Code,
			Name = request.Name,
			Description = request.Description,
		};

		var result = await _repository.InsertAsync(entity, cancellationToken);
		return result.Id;
	}

	public async Task Handle(RoleUpdateCommand request, CancellationToken cancellationToken)
	{
		var exists = await _repository.ExistsAsync(request.Code, request.Id, cancellationToken);
		if (exists)
		{
			throw new BadRequestException("角色代码已经存在");
		}

		var entity = await _repository.GetAsync(request.Id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException("角色不存在");
		}

		entity.Code = request.Code;
		entity.Name = request.Name;
		entity.Description = request.Description;

		await _repository.UpdateAsync(entity, cancellationToken);
	}

	public async Task Handle(RoleDeleteCommand request, CancellationToken cancellationToken)
	{
		await _repository.DeleteAsync(request.Id, cancellationToken)
						 .ContinueWith(t => _mediator.Publish(new RoleDeletedEvent(request.Id), cancellationToken));
	}
}