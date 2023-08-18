using Newtonsoft.Json;

namespace Woa.Sdk.Wechat;

public sealed class WechatAccessToken
{
	/// <summary>
	/// 获取到的凭证
	/// </summary>
	[JsonProperty("access_token")]
	public string Token { get; set; }

	/// <summary>
	/// 凭证有效时间，单位：秒
	/// </summary>
	[JsonProperty("expires_in")]
	public int Expiry { get; set; }

	/// <summary>
	/// 错误代码
	/// </summary>
	[JsonProperty("errcode")]
	public int ErrorCode { get; set; }

	/// <summary>
	/// 错误消息
	/// </summary>
	[JsonProperty("errmsg")]
	public string ErrorMessage { get; set; }
}
