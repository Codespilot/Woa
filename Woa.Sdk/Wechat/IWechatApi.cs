using Refit;

namespace Woa.Sdk.Wechat;

public interface IWechatApi
{
	/// <summary>
	/// 获取 Access token
	/// </summary>
	/// <param name="type"></param>
	/// <param name="appid"></param>
	/// <param name="secret"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Get("/cgi-bin/token")]
	Task<IApiResponse<WechatAccessToken>> GrantTokenAsync([Query] [AliasAs("grant_type")] string type, [Query] string appid, [Query] string secret, CancellationToken cancellationToken = default);

	/// <summary>
	/// 发送客服消息
	/// </summary>
	/// <param name="token"></param>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Post("/cgi-bin/message/custom/send")]
	Task<IApiResponse> SendCustomMessageAsync([Query] [AliasAs("access_token")] string token, [Body] WechatMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// 上传临时素材
	/// </summary>
	/// <param name="media">form-data中媒体文件标识，有filename、filelength、content-type等信息</param>
	/// <param name="token">调用接口凭证</param>
	/// <param name="type">媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb）</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Multipart]
	[Post("/cgi-bin/media/upload")]
	Task<IApiResponse> UploadTemporaryMediaAsync(StreamPart media, [Query] [AliasAs("access_token")] string token, [Query] string type, CancellationToken cancellationToken = default);

	/// <summary>
	/// 上传图文消息内的图片获取URL
	/// </summary>
	/// <param name="media">form-data中媒体文件标识，有filename、filelength、content-type等信息</param>
	/// <param name="token">调用接口凭证</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Multipart]
	[Post("/cgi-bin/media/uploadimg")]
	Task<IApiResponse> UploadImageAsync(StreamPart media, [Query] [AliasAs("access_token")] string token, CancellationToken cancellationToken = default);

	/// <summary>
	/// 新增其他类型永久素材
	/// </summary>
	/// <param name="media">form-data中媒体文件标识，有filename、filelength、content-type等信息</param>
	/// <param name="description"></param>
	/// <param name="token">调用接口凭证</param>
	/// <param name="type">媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb）</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Multipart]
	[Post("/cgi-bin/material/add_material")]
	Task<IApiResponse> UploadPermanentMediaAsync(StreamPart media, string description, [Query] [AliasAs("access_token")] string token, [Query] string type, CancellationToken cancellationToken = default);

	/// <summary>
	/// 创建自定义菜单
	/// </summary>
	/// <param name="request">菜单数据</param>
	/// <param name="token">调用接口凭证</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Post("/cgi-bin/menu/create")]
	Task<IApiResponse<WechatResponse>> CreateMenuAsync([Body] WechatMenuUpdateRequest request, [Query] [AliasAs("access_token")] string token, CancellationToken cancellationToken = default);
	
	/// <summary>
	/// 删除自定义菜单
	/// </summary>
	/// <param name="token">调用接口凭证</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	[Post("/cgi-bin/menu/delete")]
	Task<IApiResponse<WechatResponse>> DeleteMenuAsync([Query] [AliasAs("access_token")] string token, CancellationToken cancellationToken = default);
}