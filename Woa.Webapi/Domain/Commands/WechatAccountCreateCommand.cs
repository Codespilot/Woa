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
	/// 开发者密码
	/// </summary>
	public string AppSecret { get; set; }

	/// <summary>
	/// 开发者令牌
	/// </summary>
	/// <remarks>
	/// 必须为英文或数字，长度为3-32字符。
	/// </remarks>
	public string AppToken { get; set; }

	/// <summary>
	/// 消息加解密密钥
	/// </summary>
	/// <remarks>
	/// 消息加密密钥由43位字符组成，可随机修改，字符范围为A-Z，a-z，0-9。
	/// </remarks>
	public string EncryptKey { get; set; }

	/// <summary>
	/// 公众号消息加密方式
	/// </summary>
	public string EncryptType { get; set; }

	/// <summary>
	/// 公众号是否开启客服消息
	/// </summary>
	public bool EnableCustomMessage { get; set; }

	/// <summary>
	/// 公众号是否开启模板消息
	/// </summary>
	public bool EnableTemplateMessage { get; set; }

	/// <summary>
	/// 是否已认证
	/// </summary>
	public bool Verified { get; set; }

	/// <summary>
	/// 公众号消息回复内容标题
	/// </summary>
	public string ReplyTitle { get; set; }

	/// <summary>
	/// 公众号消息回复内容描述
	/// </summary>
	public string ReplyDescription { get; set; }

	/// <summary>
	/// 公众号消息回复内容查看链接
	/// </summary>
	public string ReplyUrl { get; set; }

	/// <summary>
	/// 公众号消息回复图片链接
	/// </summary>
	public string ReplyPicUrl { get; set; }
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

		RuleFor(model => model.AppToken).Cascade(CascadeMode.Stop)
		                                .NotEmpty().WithMessage("开发者令牌不能为空")
		                                .Matches(@"^[A-Za-z0-9]{3,32}$").WithMessage("开发者令牌必须为英文或数字，长度为3-32字符。");

		RuleFor(model => model.EncryptKey).Cascade(CascadeMode.Stop)
		                                  .Matches(@"^[A-Za-z0-9]{43}$").WithMessage("消息加解密密钥消息加密密钥由43位字符组成，可随机修改，字符范围为A-Z，a-z，0-9。");
	}
}