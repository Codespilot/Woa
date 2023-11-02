namespace Woa.Transit;

public abstract class WechatAccountEditDto
{
	/// <summary>
	/// 微信号
	/// </summary>
	public string Account { get; set; }

	/// <summary>
	/// 类型
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// 公众号名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 公众号简介
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 公众号头像
	/// </summary>
	public string Image { get; set; }

	/// <summary>
	/// 开发者ID
	/// </summary>
	public string AppId { get; set; }

	/// <summary>
	/// 是否已认证
	/// </summary>
	public bool Verified { get; set; }
}