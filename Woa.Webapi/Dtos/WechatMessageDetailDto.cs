namespace Woa.Webapi.Dtos;

/// <summary>
/// 微信消息明细对象
/// </summary>
public class WechatMessageDetailDto
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
	/// 发送放OpenId
	/// </summary>
	public string OpenId { get; set; }

	/// <summary>
	/// 发送时间
	/// </summary>
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 消息明细
	/// </summary>
	public object Detail { get; set; }

	/// <summary>
	/// 回复内容
	/// </summary>
	public string Reply { get; set; }
}