using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Woa.Common;
using Woa.Sdk.Tencent;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class WechatMenuApplicationService : BaseApplicationService, IWechatMenuApplicationService
{
	private IWechatApi _wechatApi;
	private WechatMenuRepository _repository;

	private IWechatApi WechatApi => _wechatApi ??= ServiceProvider.GetService<IWechatApi>();
	private WechatMenuRepository Repository => _repository ??= ServiceProvider.GetService<WechatMenuRepository>();

	public async Task<List<WechatMenuItemDto>> SearchAsync(WechatMenuQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var offset = (page - 1) * size;

		var expressions = new List<Expression<Func<WechatMenuEntity, bool>>>();
		if (!string.IsNullOrEmpty(condition.Type))
		{
			expressions.Add(t => t.Type == condition.Type);
		}

		if (condition.ParentId.HasValue)
		{
			expressions.Add(t => t.ParentId == condition.ParentId);
		}

		var predicate = expressions.Aggregate(t => t.Id > 0);

		var entities = await Repository.FindAsync(predicate, offset, size, cancellationToken);
		var result = Mapper.Map<List<WechatMenuItemDto>>(entities);
		return result;
	}

	public async Task<WechatMenuDetailDto> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		var entity = await Repository.GetAsync(id, cancellationToken);
		return Mapper.Map<WechatMenuDetailDto>(entity);
	}

	public async Task<int> CreateAsync(WechatMenuEditDto model, CancellationToken cancellationToken = default)
	{
		var command = Mapper.Map<WechatMenuCreateCommand>(model);
		return await Mediator.Send(command, cancellationToken);
	}

	public async Task UpdateAsync(int id, WechatMenuEditDto model, CancellationToken cancellationToken = default)
	{
		var command = new WechatMenuUpdateCommand(id);
		command = Mapper.Map(model, command);
		await Mediator.Send(command, cancellationToken);
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var command = new WechatMenuDeleteCommand(id);
		await Mediator.Send(command, cancellationToken);
	}

	public async Task PublishAsync(CancellationToken cancellationToken = default)
	{
		var entities = await Repository.FindAsync(t => t.IsValid, cancellationToken);

		var level1 = entities.Where(t => t.ParentId == 0)
							 .OrderBy(t => t.Sort)
							 .ThenBy(t => t.Id)
							 .ToList();

		var request = new WechatMenuUpdateRequest();

		foreach (var entity in level1)
		{
			var level2 = entities.Where(t => t.ParentId == entity.Id)
								 .OrderBy(t => t.Sort)
								 .ThenBy(t => t.Id)
								 .ToList();

			if (level2.Any())
			{
				var button = new WechatMenu
				{
					Name = entity.Name,
					Submenu = new List<WechatMenu>()
				};

				foreach (var item in level2)
				{
					var subButton = new WechatMenu
					{
						Name = item.Name,
						Type = item.Type,
						Url = item.Url,
						Key = item.Key,
						MiniAppId = item.MiniAppId,
						MiniAppPage = item.MiniAppPage
					};

					button.Submenu.Add(subButton);
				}

				request.Menus.Add(button);
			}
			else
			{
				var button = new WechatMenu
				{
					Name = entity.Name,
					Type = entity.Type,
					Url = entity.Url,
					Key = entity.Key,
					MiniAppId = entity.MiniAppId,
					MiniAppPage = entity.MiniAppPage
				};

				request.Menus.Add(button);
			}
		}

		var response = await WechatApi.CreateMenuAsync(request, Cache.Get<string>(Constants.Cache.WechatAccessToken), cancellationToken);
		if (response.IsSuccessStatusCode)
		{
			if (response.Content?.ErrorCode != 0)
			{
				Logger.LogError("自定义菜单推送失败：{Message}", response.Content?.ErrorMessage);
				throw new BadGatewayException("自定义菜单推送失败");
			}
		}
		else
		{
			Logger.LogError("自定义菜单推送失败：{Message}", response.Error?.Message);
			throw new BadGatewayException("自定义菜单推送失败");
		}
	}
}