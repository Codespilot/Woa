using Newtonsoft.Json;

namespace Woa.Sdk.Tencent;

public sealed class WechatAccessToken : WechatResponse
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
}