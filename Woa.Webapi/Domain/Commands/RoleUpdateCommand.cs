using FluentValidation;

namespace Woa.Webapi.Domain;

public sealed class RoleUpdateCommand : ICommand
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }
}

public class RoleUpdateCommandValidator: AbstractValidator<RoleUpdateCommand>
{
    public RoleUpdateCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0)
                          .WithMessage("角色ID必须大于0");

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