using Postgrest.Attributes;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

[Table("wechat_account")]
public class WechatAccountEntity : BaseModel,
                                   IEntity<string>,
                                   ICreateAudit<int>,
                                   IUpdateAudit<int?>
{
	/// <summary>
	/// 微信公众号Id（OpenId）
	/// </summary>
	[PrimaryKey("id", true)]
	public string Id { get; set; }

	/// <summary>
	/// 微信号
	/// </summary>
	[Column("account")]
	public string Account { get; set; }

	/// <summary>
	/// 类型
	/// </summary>
	[Column("type")]
	public string Type { get; set; }

	/// <summary>
	/// 公众号名称
	/// </summary>
	[Column("name")]
	public string Name { get; set; }

	/// <summary>
	/// 公众号简介
	/// </summary>
	[Column("description")]
	public string Description { get; set; }

	/// <summary>
	/// 公众号头像
	/// </summary>
	[Column("image")]
	public string Image { get; set; }

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
	/// 消息加解密密钥
	/// </summary>
	[Column("encrypt_key")]
	public string EncryptKey { get; set; }

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
	/// 是否已认证
	/// </summary>
	[Column("verified")]
	public bool Verified { get; set; }

	/// <summary>
	/// 是否有效
	/// </summary>
	[Column("is_valid")]
	public bool IsValid { get; set; }

	[Column("create_by", ignoreOnUpdate: true)]
	public int CreateBy { get; set; }

	[Column("create_at", ignoreOnUpdate: true)]
	public DateTime CreateAt { get; set; }

	[Column("update_by", ignoreOnInsert: true)]
	public int? UpdateBy { get; set; }

	[Column("update_at", ignoreOnInsert: true)]
	public DateTime? UpdateAt { get; set; }

	object ICreateAudit.CreateBy
	{
		get => CreateBy;
		set => CreateBy = value switch
		{
			int i => i,
			_ => throw new ArgumentException("CreateBy must be int")
		};
	}

	object IUpdateAudit.UpdateBy
	{
		get => UpdateBy;
		set => UpdateBy = value switch
		{
			null => null,
			int i => i,
			_ => throw new ArgumentException("UpdateBy must be int or null")
		};
	}
}