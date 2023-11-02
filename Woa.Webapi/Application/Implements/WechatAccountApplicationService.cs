using System.Linq.Expressions;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class WechatAccountApplicationService : BaseApplicationService, IWechatAccountApplicationService
{
	private WechatAccountRepository _repository;
	private WechatAccountRepository Repository => _repository ??= ServiceProvider.GetService<WechatAccountRepository>();

	public Task<List<WechatAccountItemDto>> SearchAsync(WechatAccountQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		return Repository.FindAsync(predicate, page, size, cancellationToken)
		                 .ContinueWith(t => Mapper.Map<List<WechatAccountItemDto>>(t.Result), cancellationToken);
	}

	public Task<int> CountAsync(WechatAccountQueryDto condition, CancellationToken cancellationToken = default)
	{
		var predicate = BuildExpression(condition);

		return Repository.CountAsync(predicate, cancellationToken);
	}

	public Task<WechatAccountDetailDto> GetAsync(string id, CancellationToken cancellationToken = default)
	{
		return Repository.GetAsync(id, cancellationToken)
		                 .ContinueWith(t => Mapper.Map<WechatAccountDetailDto>(t.Result), cancellationToken);
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

	public Task SetValidityAsync(string id, bool validity, CancellationToken cancellationToken = default)
	{
		var command = new WechatAccountSetValidityCommand(id, validity);
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

		if (condition.IsValid.HasValue)
		{
			expressions.Add(t => t.IsValid == condition.IsValid.Value);
		}

		if (!string.IsNullOrWhiteSpace(condition.Keyword))
		{
			expressions.Add(t => t.Name.Contains(condition.Keyword) || t.Account.Contains(condition.Keyword) || t.Description.Contains(condition.Keyword));
		}

		return expressions.Aggregate(t => t.Id != null);
	}

	#endregion
}