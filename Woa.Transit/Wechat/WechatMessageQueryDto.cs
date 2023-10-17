namespace Woa.Transit;

/// <summary>
/// 微信消息查询对象
/// </summary>
internal class WechatMessageQueryDto
{
	/// <summary>
	/// 消息类型
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// 用户open id
	/// </summary>
	public string OpenId { get; set; }

	/// <summary>
	/// 微信公众号Id
	/// </summary>
	public string AccountId { get; set; }
}
