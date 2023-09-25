using FluentValidation;

namespace Woa.Webapi.Domain;

public record WechatMessageReplyCommand(long Id, string Content) : ICommand;

public class WechatMessageReplyCommandValidator : AbstractValidator<WechatMessageReplyCommand>
{
	public WechatMessageReplyCommandValidator()
	{
		RuleFor(command => command.Content).NotEmpty()
								   .WithMessage("回复内容不能为空")
								   .MaximumLength(1000)
								   .WithMessage("回复内容不能超过1000个字");
	}
}