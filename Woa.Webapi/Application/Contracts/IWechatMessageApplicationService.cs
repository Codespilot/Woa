using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public interface IWechatMessageApplicationService : IApplicationService
{
	/// <summary>
	/// 搜索微信消息
	/// </summary>
	/// <param name="condition">搜索条件</param>
	/// <param name="page">页码</param>
	/// <param name="size">数量</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns>符合条件的微信消息列表对象集合</returns>
	Task<List<WechatMessageItemDto>> SearchAsync(WechatMenuQueryDto condition, int page, int size, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取微信消息详情
	/// </summary>
	/// <param name="id">微信消息Id</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns></returns>
	Task<WechatMessageDetailDto> GetAsync(long id, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取Chatbot回复
	/// </summary>
	/// <param name="id">微信消息Id</param>
	/// <param name="cancellationToken">操作取消令牌</param>
	/// <returns></returns>
	Task<string> GetReplyAsync(long id, CancellationToken cancellationToken = default);
}