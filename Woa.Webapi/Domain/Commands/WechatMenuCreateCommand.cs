using FluentValidation;
using Woa.Sdk.Tencent;

namespace Woa.Webapi.Domain;

public class WechatMenuCreateCommand : ICommand<int>
{
	/// <summary>
	/// 上级菜单Id
	/// </summary>
	public int ParentId { get; set; }

	/// <summary>
	/// 菜单的响应动作类型，view表示网页类型，click表示点击类型，miniprogram表示小程序类型
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// 菜单标题，不超过16个字节，子菜单不超过60个字节
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 菜单KEY值，用于消息接口推送，不超过128字节
	/// </summary>
	public string Key { get; set; }

	/// <summary>
	/// 网页 链接，用户点击菜单可打开链接，不超过1024字节。
	/// type为miniprogram时，不支持小程序的老版本客户端将打开本url。
	/// </summary>
	public string Url { get; set; }

	/// <summary>
	/// 小程序的appid（仅认证公众号可配置）
	/// </summary>
	public string MiniAppId { get; set; }

	/// <summary>
	/// 小程序的页面路径
	/// </summary>
	public string MiniAppPage { get; set; }

	/// <summary>
	/// 排序
	/// </summary>
	public int Sort { get; set; }
}

public class WechatMenuCreateCommandValidator : AbstractValidator<WechatMenuCreateCommand>
{
	private static readonly string[] _clickTypes = { WechatMenuType.Click, WechatMenuType.PickPhoto, WechatMenuType.PickPhotoOrAlbum, WechatMenuType.PickWeixinPhoto, WechatMenuType.LocationSelect };

	public WechatMenuCreateCommandValidator()
	{
		When(command => command.ParentId == 0, () =>
		{
			RuleFor(t => t.Name).NotEmpty().WithMessage("菜单标题不能为空")
			                    .MaximumLength(16).WithMessage("一级菜单标题不能超过16个字节");
		}).Otherwise(() =>
		{
			// 二级菜单类型必填
			RuleFor(t => t.Type).NotEmpty()
			                    .WithMessage("菜单的响应动作类型不能为空")
			                    .Must(t => WechatMenuType.GetValues().Contains(t))
			                    .WithMessage("菜单的响应动作类型不正确");
			RuleFor(t => t.Name).NotEmpty().WithMessage("菜单标题不能为空")
			                    .MaximumLength(60).WithMessage("二级菜单标题不能超过60个字节");
		});

		When(command => _clickTypes.Contains(command.Type), () =>
		{
			RuleFor(t => t.Key).NotEmpty().WithMessage("菜单KEY值不能为空")
			                   .MaximumLength(128).WithMessage("菜单KEY值不能超过128个字节")
			                   .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("菜单KEY值只能是字母、数字、下划线、中划线组成")
			                   .Must(t => t.Length <= 128).WithMessage("菜单KEY值不能超过128个字节")
			                   .Must(t => t.Length >= 1).WithMessage("菜单KEY值不能少于1个字节");
		});

		When(command => command.Type == WechatMenuType.View, () =>
		{
			RuleFor(t => t.Url).NotEmpty().WithMessage("网页链接不能为空");
		});

		When(command => command.Type == WechatMenuType.MiniApp, () =>
		{
			RuleFor(t => t.MiniAppId).NotEmpty().WithMessage("小程序Id不能为空");
			RuleFor(t => t.MiniAppPage).NotEmpty().WithMessage("小程序页面不能为空");
			RuleFor(t => t.Url).NotEmpty().WithMessage("网页链接不能为空");
		});
	}
}