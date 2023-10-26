using Woa.Transit;

namespace Woa.Webapi.Application;

public interface IUserApplicationService : IApplicationService
{
	/// <summary>
	/// 用户认证
	/// </summary>
	/// <param name="username"></param>
	/// <param name="password"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<LoginResponseDto> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

	/// <summary>
	/// 用户认证
	/// </summary>
	/// <param name="token"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<LoginResponseDto> AuthenticateAsync(string token, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取用户详细信息
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<UserDetailDto> GetAsync(int id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取用户详细信息
	/// </summary>
	/// <param name="username"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<UserDetailDto> GetAsync(string username, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取用户简要信息
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<UserProfileDto> GetProfileAsync(int id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 新增用户
	/// </summary>
	/// <param name="model"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<int> CreateAsync(UserCreateDto model, CancellationToken cancellationToken = default);

	/// <summary>
	/// 查询用户列表
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="page"></param>
	/// <param name="size"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<List<UserItemDto>> SearchAsync(UserQueryDto condition, int page, int size, CancellationToken cancellationToken = default);

	/// <summary>
	/// 查询用户数量
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<int> CountAsync(UserQueryDto condition, CancellationToken cancellationToken = default);
}