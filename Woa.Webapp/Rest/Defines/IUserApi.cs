using Refit;
using Woa.Transit;

namespace Woa.Webapp.Rest;

internal interface IUserApi
{
	[Get("/api/user/list")]
	Task<IApiResponse<List<UserItemDto>>> SearchAsync([Query] UserQueryDto condition, [Query] int page, [Query] int size, CancellationToken cancellationToken = default);

	[Get("/api/user/count")]
	Task<IApiResponse<int>> CountAsync([Query] UserQueryDto condition, CancellationToken cancellationToken = default);

	[Get("/api/user/{id}")]
	Task<IApiResponse<UserDetailDto>> GetAsync(int id, CancellationToken cancellationToken = default);

	[Post("/api/user")]
	Task<IApiResponse> CreateAsync([Body] UserCreateDto model, CancellationToken cancellationToken = default);

	[Put("/api/user/{id}")]
	Task<IApiResponse> UpdateAsync(int id, [Body] UserUpdateDto model, CancellationToken cancellationToken = default);

	[Put("/api/user/{id}/role")]
	Task<IApiResponse> UpdateRoleAsync(int id, [Body] List<int> model, CancellationToken cancellationToken = default);
}