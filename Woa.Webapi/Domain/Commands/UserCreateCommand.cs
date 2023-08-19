using FluentValidation;

namespace Woa.Webapi.Domain;

public sealed class UserCreateCommand : ICommand<int>
{
	public string Username { get; set; }

	public string Password { get; set; }

	public string Email { get; set; }

	public string Phone { get; set; }
}

public sealed class UserCreateCommandValidator : AbstractValidator<UserCreateCommand>
{
	public UserCreateCommandValidator()
	{
		RuleFor(x => x.Username).NotEmpty().WithMessage("用户名不能为空。")
		                        .MaximumLength(50).WithMessage("用户名长度不能超过50个字符。");

		RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空")
		                        .MaximumLength(50).WithMessage("密码长度不能超过50个字符。");

		When(model => !string.IsNullOrEmpty(model.Email), () =>
		{
			RuleFor(x => x.Email).EmailAddress().WithMessage("邮箱格式不正确。")
			                     .MaximumLength(255).WithMessage("邮箱长度不能超过50个字符。");
		});

		When(model => !string.IsNullOrEmpty(model.Phone), () =>
		{
			RuleFor(x => x.Phone).MaximumLength(50).WithMessage("手机号码长度不能超过50个字符。");
		});
	}
}