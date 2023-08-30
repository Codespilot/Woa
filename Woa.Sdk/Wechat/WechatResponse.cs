using Newtonsoft.Json;

namespace Woa.Sdk.Wechat;

public class WechatResponse
{
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