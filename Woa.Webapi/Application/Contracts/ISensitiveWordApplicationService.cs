using Woa.Transit;

namespace Woa.Webapi.Application;

/// <summary>
/// 敏感词应用服务
/// </summary>
public interface ISensitiveWordApplicationService : IApplicationService
{
	/// <summary>
	/// 搜索敏感词
	/// </summary>
	/// <param name="keyword">关键字</param>
	/// <param name="page">页码</param>
	/// <param name="size">每页数量</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<List<SensitiveWordItemDto>> SearchAsync(string keyword, int page, int size, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取敏感词数量
	/// </summary>
	/// <param name="keyword">关键字</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<int> CountAsync(string keyword, CancellationToken cancellationToken = default);

	/// <summary>
	/// 新增敏感词
	/// </summary>
	/// <param name="request"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task CreateAsync(SensitiveWordCreateDto request, CancellationToken cancellationToken = default);

	/// <summary>
	/// 删除敏感词
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}