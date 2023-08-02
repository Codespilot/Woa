using Refit;
using Woa.Webapi.Models;

namespace Woa.Webapi.Apis;

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
}