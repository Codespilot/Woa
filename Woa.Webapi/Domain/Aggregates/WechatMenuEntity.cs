using Postgrest.Attributes;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

/// <summary>
/// 微信公众号自定义菜单
/// </summary>
[Table("wechat_menus")]
public class WechatMenuEntity : BaseModel, IEntity<int>
{
	/// <summary>
	/// 菜单主键
	/// </summary>
	[PrimaryKey("id")]
	public int Id { get; set; }

	/// <summary>
	/// 上级菜单Id
	/// </summary>
	[Column("parent_id")]
	public int ParentId { get; set; }

	/// <summary>
	/// 公众号Id
	/// </summary>
	[Column("platform_id")]
	public string PlatformId { get; set; }

	/// <summary>
	/// 菜单的响应动作类型，view表示网页类型，click表示点击类型，miniprogram表示小程序类型
	/// </summary>
	[Column("type")]
	public string Type { get; set; }

	/// <summary>
	/// 菜单标题，不超过16个字节，子菜单不超过60个字节
	/// </summary>
	[Column("name")]
	public string Name { get; set; }

	/// <summary>
	/// 菜单KEY值，用于消息接口推送，不超过128字节
	/// </summary>
	[Column("key")]
	public string Key { get; set; }

	/// <summary>
	/// 网页链接，用户点击菜单可打开链接，不超过1024字节。
	/// type为miniprogram时，不支持小程序的老版本客户端将打开本url。
	/// </summary>
	[Column("url")]
	public string Url { get; set; }

	/// <summary>
	/// 小程序的appid（仅认证公众号可配置）
	/// </summary>
	[Column("mini_app_id")]
	public string MiniAppId { get; set; }

	/// <summary>
	/// 小程序的页面路径
	/// </summary>
	[Column("mini_app_page")]
	public string MiniAppPage { get; set; }

	/// <summary>
	/// 排序
	/// </summary>
	[Column("sort")]
	public int Sort { get; set; }
	
	/// <summary>
	/// 是否有效
	/// </summary>
	[Column("is_valid")]
	public bool IsValid { get; set; }

	/// <summary>
	/// 创建时间
	/// </summary>
	[Column("create_time")]
	public DateTime CreateTime { get; set; }
	
	/// <summary>
	/// 更新时间
	/// </summary>
	[Column("update_time")]
	public DateTime? UpdateTime { get; set; }
}