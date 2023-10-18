namespace Woa.Transit;

/// <summary>
/// 微信消息列表对象
/// </summary>
public class WechatMessageItemDto
{
	/// <summary>
	/// 消息Id
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// 消息类型
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// 用户Open Id
	/// </summary>
	public string OpenId { get; set; }

	/// <summary>
	/// 消息发送时间
	/// </summary>
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 是否有回复
	/// </summary>
	public bool HasReply { get; set; }
}
