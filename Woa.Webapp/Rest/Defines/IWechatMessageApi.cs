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
	[Get("/api/wechat/message")]
	Task<IApiResponse<List<WechatMessageItemDto>>> SearchAsync([Query] WechatMessageQueryDto condition, [Query] int page = 1, [Query] int size = 20, CancellationToken cancellationToken = default);
}