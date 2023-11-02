using System.Linq.Expressions;
using Woa.Common;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class WechatAccountApplicationService : BaseApplicationService, IWechatAccountApplicationService
{
	private WechatAccountRepository _repository;
	private WechatAccountRepository Repository => _repository ??= ServiceProvider.GetService<WechatAccountRepository>();

	public async Task<List<WechatAccountItemDto>> SearchAsync(WechatAccountQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		var entities = await Repository.FindAsync(predicate, page, size, cancellationToken);

		var userIds = entities.SelectMany(t => new[] { t.CreateBy, t.UpdateBy ?? 0 })
							  .Where(t => t > 0)
							  .Distinct()
							  .ToList();

		var users = await ServiceProvider.GetService<CommonRepository>().GetUserNameAsync(userIds, t => t.Username, cancellationToken);

		return Mapper.Map<List<WechatAccountItemDto>>(entities, opts => opts.Items["users"] = users);
	}

	public Task<int> CountAsync(WechatAccountQueryDto condition, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		return Repository.CountAsync(predicate, cancellationToken);
	}

	public async Task<WechatAccountDetailDto> GetAsync(string id, CancellationToken cancellationToken = default)
	{
		var entity = await Repository.GetAsync(id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException("公众号配置不存在");
		}

		var users = await ServiceProvider.GetService<CommonRepository>().GetUserNameAsync(new[] { entity.CreateBy, entity.UpdateBy ?? 0 }, t => t.Username, cancellationToken);

		return Mapper.Map<WechatAccountDetailDto>(entity, opts => opts.Items["users"] = users);
	}

	public Task<string> CreateAsync(WechatAccountCreateDto model, CancellationToken cancellationToken = default)
	{
		var command = Mapper.Map<WechatAccountCreateCommand>(model);
		return Mediator.Send(command, cancellationToken)
					   .ContinueWith(t => t.Result, cancellationToken);
	}

	public Task UpdateAsync(string id, WechatAccountUpdateDto model, CancellationToken cancellationToken = default)
	{
		var command = new WechatAccountUpdateCommand(id);
		Mapper.Map(model, command);
		return Mediator.Send(command, cancellationToken);
	}

	#region Supports

	private static Expression<Func<WechatAccountEntity, bool>> BuildExpression(WechatAccountQueryDto condition)
	{
		var expressions = new List<Expression<Func<WechatAccountEntity, bool>>>();
		if (!string.IsNullOrWhiteSpace(condition.Type))
		{
			expressions.Add(t => t.Type == condition.Type);
		}
		
		if (!string.IsNullOrWhiteSpace(condition.Keyword))
		{
			expressions.Add(t => t.Name.Contains(condition.Keyword) || t.Account.Contains(condition.Keyword) || t.Description.Contains(condition.Keyword));
		}

		return expressions.Aggregate(t => t.Id != null);
	}
	#endregion
}