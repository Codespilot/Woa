using Postgrest.Attributes;
using Postgrest.Models;

// ReSharper disable ExplicitCallerInfoArgument

namespace Woa.Webapi.Domain;

[Table("wechat_follower")]
public class WechatFollowerEntity : BaseModel, IEntity<long>
{
	/// <summary>
	/// Id
	/// </summary>
	[PrimaryKey]
	public long Id { get; set; }

	/// <summary>
	/// 用户微信账号Id
	/// </summary>
	[Column("open_id")]
	public string OpenId { get; set; }

	/// <summary>
	/// 微信公众号Id
	/// </summary>
	[Column("platform_id")]
	public string PlatformId { get; set; }

	/// <summary>
	/// 用户昵称
	/// </summary>
	[Column("nick_name")]
	public string Nickname { get; set; } = string.Empty;

	[Column("create_time", ignoreOnUpdate: true)]
	public DateTime CreateTime { get; set; }

	/// <summary>
	/// 关注时间
	/// </summary>
	[Column("subscribe_time")]
	public DateTime SubscribeTime { get; set; }

	/// <summary>
	/// 取消关注时间
	/// </summary>
	[Column("unsubscribe_time")]
	public DateTime? UnsubscribeTime { get; set; }

	[Column("state")]
	public int State { get; set; }

	/// <summary>
	/// 是否开启机器人对话
	/// </summary>
	[Column("chatbot_enabled")]
	public bool IsChatbotEnabled { get; set; }
}