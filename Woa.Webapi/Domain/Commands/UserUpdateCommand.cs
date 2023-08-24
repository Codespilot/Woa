using MediatR;

namespace Woa.Webapi.Domain;

/// <summary>
/// 用户更新Command
/// </summary>
public class UserUpdateCommand : ICommand<Unit>
{
	public string Fullname { get; set; }
	
	public string Email { get; set; }

	public string Phone { get; set; }
}