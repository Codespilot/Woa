using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public interface ISensitiveWordApplicationService
{
	Task<List<SensitiveWordItemDto>> SearchAsync(string keyword, int page, int size, CancellationToken cancellationToken = default);

	Task<int> CountAsync(string keyword, CancellationToken cancellationToken = default);
	
	Task CreateAsync(SensitiveWordCreateDto request, CancellationToken cancellationToken = default);

	Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}