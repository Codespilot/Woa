using Woa.Transit;

namespace Woa.Webapi.Application;

/// <summary>
/// 微信自定义菜单应用服务接口
/// </summary>
public interface IWechatMenuApplicationService : IApplicationService
{
	/// <summary>
	/// 搜索微信自定义菜单
	/// </summary>
	/// <param name="condition">搜索条件</param>
	/// <param name="page">页码</param>
	/// <param name="size">数量</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns>自定义菜单列表对象集合</returns>
	Task<List<WechatMenuItemDto>> SearchAsync(WechatMenuQueryDto condition, int page, int size, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取微信自定义菜单详情
	/// </summary>
	/// <param name="id">自定义菜单Id</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns>自定义菜单详情对象</returns>
	Task<WechatMenuDetailDto> GetAsync(int id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 新增微信自定义菜单
	/// </summary>
	/// <param name="model">自定义菜单模型</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns></returns>
	Task<int> CreateAsync(WechatMenuEditDto model, CancellationToken cancellationToken = default);

	/// <summary>
	/// 更新自定义菜单
	/// </summary>
	/// <param name="id">自定义菜单Id</param>
	/// <param name="model">自定义菜单模型</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns></returns>
	Task UpdateAsync(int id, WechatMenuEditDto model, CancellationToken cancellationToken = default);

	/// <summary>
	/// 删除自定义菜单
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns></returns>
	Task DeleteAsync(int id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 发布自定义菜单
	/// </summary>
	/// <remarks>
	///		通过微信公众号自定义菜单创建接口推送
	/// </remarks>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns></returns>
	Task PublishAsync(CancellationToken cancellationToken = default);
}