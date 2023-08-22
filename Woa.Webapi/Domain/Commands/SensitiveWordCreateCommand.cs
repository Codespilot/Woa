using FluentValidation;

namespace Woa.Webapi.Domain;

public class SensitiveWordCreateCommand : ICommand
{
	public SensitiveWordCreateCommand(string content)
	{
		Content = content;
	}

	public string Content { get; }
}

public class SensitiveWordCreateCommandValidator : AbstractValidator<SensitiveWordCreateCommand>
{
	public SensitiveWordCreateCommandValidator()
	{
		RuleFor(t => t.Content).NotEmpty().WithMessage("敏感词不能为空");
	}
}