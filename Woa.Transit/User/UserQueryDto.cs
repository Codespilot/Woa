namespace Woa.Transit;

/// <summary>
/// 用户查询条件
/// </summary>
public class UserQueryDto
{
	/// <summary>
	/// 关键字
	/// </summary>
	public string Keyword { get; set; }

	/// <summary>
	/// 是否锁定
	/// </summary>
	public bool? Locked { get; set; }
}
