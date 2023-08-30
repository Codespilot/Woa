using Postgrest.Attributes;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

[Table("wechat_message")]
public class WechatMessageEntity : BaseModel, IEntity<long>
{
	/// <summary>
	/// 
	/// </summary>
	[PrimaryKey("id", shouldInsert: true)]
	public long Id { get; set; }

	/// <summary>
	/// 发送方账号
	/// </summary>
	[Column("open_id")]
	public string OpenId { get; set; }

	/// <summary>
	/// 消息类型
	/// </summary>
	[Column("type")]
	public string Type { get; set; }

	/// <summary>
	/// 消息创建时间
	/// </summary>
	[Column("create_time")]
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 消息完整内容
	/// </summary>
	[Column("payload")]
	public string Payload { get; set; }

	[Column("reply")]
	public string Reply { get; set; }
}