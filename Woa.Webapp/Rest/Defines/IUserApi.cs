using Refit;
using Woa.Transit;

namespace Woa.Webapp.Rest;

internal interface IUserApi
{
	[Get("/api/user")]
	Task<IApiResponse<List<UserItemDto>>> SearchAsync([Query] UserQueryDto condition, [Query] int page, [Query] int size, CancellationToken cancellationToken = default);

	[Get("/api/user/{id}")]
	Task<IApiResponse<UserDetailDto>> GetAsync(int id, CancellationToken cancellationToken = default);

	[Post("/api/user")]
	Task<IApiResponse> CreateAsync([Body] UserCreateDto model, CancellationToken cancellationToken = default);
}