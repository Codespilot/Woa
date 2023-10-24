using Refit;
using Woa.Transit;

namespace Woa.Webapp.Rest;

public interface IRoleApi
{
	[Get("/api/role")]
	Task<IApiResponse<List<RoleInfoDto>>> SearchAsync([Query] RoleQueryDto condition, [Query] int page, [Query] int size, CancellationToken cancellationToken = default);

	[Get("/api/role/{id}")]
	Task<IApiResponse<RoleInfoDto>> GetAsync(int id, CancellationToken cancellationToken = default);

	[Post("/api/role")]
	Task<IApiResponse> CreateAsync([Body] RoleEditDto model, CancellationToken cancellationToken = default);

	[Put("/api/role/{id}")]
	Task<IApiResponse> UpdateAsync(int id, [Body] RoleEditDto model, CancellationToken cancellationToken = default);

	[Delete("/api/role/{id}")]
	Task<IApiResponse> DeleteAsync(int id, CancellationToken cancellationToken = default);
}