using Refit;
using Woa.Transit;

namespace Woa.Webapp.Rest;

internal interface IWechatMessageApi
{
	/// <summary>
	/// 搜索微信消息
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="page"></param>
	/// <param name="size"></param>
	/// <param name="cancellationToken"></param>	
	/// <returns></returns>
	[Get("/api/wechat/message/list")]
	Task<IApiResponse<List<WechatMessageItemDto>>> SearchAsync([Query] WechatMessageQueryDto condition, [Query] int page = 1, [Query] int size = 20, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取微信消息数量
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Get("/api/wechat/message/count")]
	Task<IApiResponse<int>> CountAsync([Query] WechatMessageQueryDto condition, CancellationToken cancellationToken = default);

	/// <summary>
	/// 获取微信消息详情
	/// </summary>
	/// <param name="id"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Get("/api/wechat/message/{id}")]
	Task<IApiResponse<WechatMessageDetailDto>> GetAsync(long id, CancellationToken cancellationToken = default);
}