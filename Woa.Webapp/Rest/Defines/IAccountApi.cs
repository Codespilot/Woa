using Refit;
using Woa.Transit;

namespace Woa.Webapp.Rest;

internal interface IAccountApi
{
	/// <summary>
	/// 获取Token
	/// </summary>
	/// <param name="model"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Post("/api/account/login")]
	Task<IApiResponse<LoginResponseDto>> GrantAsync([Body] LoginRequestDto model, CancellationToken cancellationToken = default);

	/// <summary>
	/// 刷新Token
	/// </summary>
	/// <param name="token"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Get("/api/account/refresh")]
	Task<IApiResponse<LoginResponseDto>> RefreshAsync(string token, CancellationToken cancellationToken = default);
}
