using Postgrest.Attributes;

namespace Woa.Webapi.Domain;

[Table("wechat_accounts")]
public class WechatAccountEntity : IEntity<string>
{
	/// <summary>
	/// 微信公众号Id（OpenId）
	/// </summary>
	[PrimaryKey("id", true)]
	public string Id { get; set; }

	/// <summary>
	/// 公众号名称
	/// </summary>
	[Column("name")]
	public string Name { get; set; }

	/// <summary>
	/// 公众号AppId
	/// </summary>
	[Column("app_id")]
	public string AppId { get; set; }

	/// <summary>
	/// 公众号AppSecret
	/// </summary>
	[Column("app_secret")]
	public string AppSecret { get; set; }

	/// <summary>
	/// 公众号自定义Token
	/// </summary>
	[Column("app_token")]
	public string AppToken { get; set; }

	/// <summary>
	/// 消息加解密密钥
	/// </summary>
	[Column("encoding_key")]
	public string EncodingKey { get; set; }

	/// <summary>
	/// 公众号消息加密方式
	/// </summary>
	[Column("encrypt_type")]
	public string EncryptType { get; set; }

	/// <summary>
	/// 公众号是否开启客服消息
	/// </summary>
	[Column("enable_custom_message")]
	public bool EnableCustomMessage { get; set; }

	/// <summary>
	/// 公众号是否开启模板消息
	/// </summary>
	[Column("enable_template_message")]
	public bool EnableTemplateMessage { get; set; }

	/// <summary>
	/// 公众号消息回复内容标题
	/// </summary>
	[Column("reply_title")]
	public string ReplyTitle { get; set; }

	/// <summary>
	/// 公众号消息回复内容描述
	/// </summary>
	[Column("reply_description")]
	public string ReplyDescription { get; set; }

	/// <summary>
	/// 公众号消息回复内容查看链接
	/// </summary>
	[Column("reply_url")]
	public string ReplyUrl { get; set; }

	/// <summary>
	/// 公众号消息回复图片链接
	/// </summary>
	[Column("reply_pic_url")]
	public string ReplyPicUrl { get; set; }

	/// <summary>
	/// 最后更新用户Id
	/// </summary>
	[Column("update_user_id", ignoreOnInsert: true)]
	public long? UpdateUserId { get; set; }

	/// <summary>
	/// 最后更新时间
	/// </summary>
	[Column("update_time", ignoreOnInsert: true)]
	public DateTime? UpdateTime { get; set; }
}
