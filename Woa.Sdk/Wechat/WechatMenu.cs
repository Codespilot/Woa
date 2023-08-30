using Newtonsoft.Json;

namespace Woa.Sdk.Wechat;

public class WechatMenu
{
	[JsonProperty("type")]
	public string Type { get; set; }

	/// <summary>
	/// 菜单标题，不超过16个字节，子菜单不超过60个字节
	/// </summary>
	[JsonProperty("name")]
	public string Name { get; set; }

	/// <summary>
	/// 菜单KEY值，用于消息接口推送，不超过128字节
	/// </summary>
	[JsonProperty("key")]
	public string Key { get; set; }

	/// <summary>
	/// 网页链接，用户点击菜单可打开链接，不超过1024字节。
	/// type为miniprogram时，不支持小程序的老版本客户端将打开本url。
	/// </summary>
	[JsonProperty("url")]
	public string Url { get; set; }

	/// <summary>
	/// 小程序的appid（仅认证公众号可配置）
	/// </summary>
	[JsonProperty("appid")]
	public string MiniAppId { get; set; }

	/// <summary>
	/// 小程序的页面路径
	/// </summary>
	[JsonProperty("pagepath")]
	public string MiniAppPage { get; set; }

	/// <summary>
	/// 二级菜单数组
	/// </summary>
	[JsonProperty("sub_button")]
	public List<WechatMenu> Submenu { get; set; }
}