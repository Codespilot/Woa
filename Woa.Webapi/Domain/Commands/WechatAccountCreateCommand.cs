using FluentValidation;

namespace Woa.Webapi.Domain;

public class WechatAccountCreateCommand : ICommand<string>
{
	/// <summary>
	/// 原始ID
	/// </summary>
	public string Id { get; set; }

	/// <summary>
	/// 微信号
	/// </summary>
	public string Account { get; set; }

	/// <summary>
	/// 类型
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// 公众号名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 公众号简介
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 公众号头像
	/// </summary>
	public string Image { get; set; }

	/// <summary>
	/// 开发者ID
	/// </summary>
	public string AppId { get; set; }

	/// <summary>
	/// 是否已认证
	/// </summary>
	public bool Verified { get; set; }
}

public class WechatAccountCreateCommandValidator : AbstractValidator<WechatAccountCreateCommand>
{
	public WechatAccountCreateCommandValidator()
	{
		RuleFor(model => model.Id).Cascade(CascadeMode.Stop)
		                          .NotEmpty().WithMessage("公众号原始ID不能为空")
		                          .Matches(@"^gh_[A-Za-z0-9]{10,20}$").WithMessage("公众号原始ID格式错误");

		RuleFor(model => model.AppId).Cascade(CascadeMode.Stop)
		                             .NotEmpty().WithMessage("开发者ID不能为空")
		                             .Matches(@"^[A-Za-z0-9]{10,20}$").WithMessage("开发者ID格式错误");
	}
}