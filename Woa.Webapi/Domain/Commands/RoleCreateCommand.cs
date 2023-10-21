using FluentValidation;

namespace Woa.Webapi.Domain;

public sealed class RoleCreateCommand : ICommand<int>
{
    public string Code { get; set; }

    public string Name { get; set; }
}

public class RoleCreateCommandValidator : AbstractValidator<RoleCreateCommand>
{
    public RoleCreateCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty()
							.WithMessage("角色代码不能为空")
                            .Matches(@"^[a-zA-Z0-9_]{2,50}$")
                            .WithMessage("角色代码只能是字母、数字、下划线组成，长度为2-50");
                            
        RuleFor(x => x.Name).NotEmpty()
							.WithMessage("角色名称不能为空")
                            .Length(2, 50)
                            .WithMessage("角色名称长度为2-50");
    }
}