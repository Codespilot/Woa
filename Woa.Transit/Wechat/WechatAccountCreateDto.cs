namespace Woa.Transit;
public class WechatAccountCreateDto : WechatAccountEditDto
{
	/// <summary>
	/// 原始ID
	/// </summary>
	public string Id { get; set; }

	/// <summary>
	/// 开发者ID
	/// </summary>
	public string AppId { get; set; }
}