using MediatR;
using Woa.Common;

namespace Woa.Webapi.Domain;

public class WechatMenuCommandHandler : ICommandHandler<WechatMenuCreateCommand, int>,
                                        ICommandHandler<WechatMenuUpdateCommand>,
                                        ICommandHandler<WechatMenuDeleteCommand>
{
	private readonly IRepository<WechatMenuEntity, int> _repository;
	private readonly IMediator _mediator;

	public WechatMenuCommandHandler(IRepository<WechatMenuEntity, int> repository, IMediator mediator)
	{
		_repository = repository;
		_mediator = mediator;
	}

	public async Task<int> Handle(WechatMenuCreateCommand request, CancellationToken cancellationToken)
	{
		if (request.ParentId > 0)
		{
			var parent = await _repository.GetAsync(t => t.Id == request.ParentId, cancellationToken);
			if (parent == null)
			{
				throw new BadRequestException("上级菜单不存在");
			}

			if (!parent.IsValid)
			{
				throw new BadRequestException("上级菜单已被禁用");
			}

			if (parent.ParentId > 0)
			{
				throw new BadRequestException("不能在二级菜单新增子菜单");
			}

			var count = await _repository.CountAsync(t => t.ParentId == request.ParentId && t.IsValid, cancellationToken);
			if (count >= 5)
			{
				throw new BadRequestException("二级菜单最多只能有5个");
			}
		}
		else
		{
			var count = await _repository.CountAsync(t => t.ParentId == 0 && t.IsValid, cancellationToken);
			if (count >= 3)
			{
				throw new BadRequestException("一级菜单最多只能有3个");
			}
		}

		var entity = new WechatMenuEntity
		{
			ParentId = request.ParentId,
			Type = request.Type,
			Name = request.Name,
			Key = request.Key,
			Url = request.Url,
			MiniAppId = request.MiniAppId,
			MiniAppPage = request.MiniAppPage,
			IsValid = true,
			CreateTime = DateTime.Now
		};

		if (request.Sort <= 0)
		{
			entity.Sort = await GetMaximumSortAsync(request.ParentId);
		}
		else
		{
			entity.Sort = request.Sort;
		}

		await _repository.InsertAsync(entity, cancellationToken);

		await _mediator.Publish(new WechatMenuChangedEvent(), cancellationToken);

		return entity.Id;
	}

	public async Task Handle(WechatMenuUpdateCommand request, CancellationToken cancellationToken)
	{
		var entity = await _repository.GetAsync(request.Id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException("菜单不存在");
		}

		if (request.ParentId > 0)
		{
			var parent = await _repository.GetAsync(t => t.Id == request.ParentId, cancellationToken);
			if (parent == null)
			{
				throw new BadRequestException("上级菜单不存在");
			}

			if (!parent.IsValid)
			{
				throw new BadRequestException("上级菜单已被禁用");
			}

			if (parent.ParentId > 0)
			{
				throw new BadRequestException("不能在二级菜单新增子菜单");
			}

			var count = await _repository.CountAsync(t => t.ParentId == request.ParentId && t.IsValid, cancellationToken);
			if (count >= 5)
			{
				throw new BadRequestException("二级菜单最多只能有5个");
			}
		}
		else
		{
			var count = await _repository.CountAsync(t => t.ParentId == 0 && t.IsValid, cancellationToken);
			if (count >= 3)
			{
				throw new BadRequestException("一级菜单最多只能有3个");
			}
		}

		entity.ParentId = request.ParentId;
		entity.Type = request.Type;
		entity.Name = request.Name;
		entity.Key = request.Key;
		entity.Url = request.Url;
		entity.MiniAppId = request.MiniAppId;
		entity.MiniAppPage = request.MiniAppPage;
		entity.UpdateTime = DateTime.Now;

		if (request.Sort <= 0)
		{
			entity.Sort = await GetMaximumSortAsync(request.ParentId);
		}
		else
		{
			entity.Sort = request.Sort;
		}

		await _repository.UpdateAsync(entity, cancellationToken);

		await _mediator.Publish(new WechatMenuChangedEvent(), cancellationToken);
	}

	public async Task Handle(WechatMenuDeleteCommand request, CancellationToken cancellationToken)
	{
		var entity = await _repository.GetAsync(request.Id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException("菜单不存在");
		}

		entity.IsValid = false;
		entity.UpdateTime = DateTime.Now;

		await _repository.UpdateAsync(entity, cancellationToken);

		await _mediator.Publish(new WechatMenuChangedEvent(), cancellationToken);
	}

	private async Task<int> GetMaximumSortAsync(int parentId)
	{
		var entities = await _repository.FindAsync(t => t.ParentId == parentId, 0, 1, t => t.Sort, false);
		if (entities.Count == 0)
		{
			return 0;
		}

		return entities.Max(t => t.Sort);
	}
}