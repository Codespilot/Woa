namespace Woa.Transit;

/// <summary>
/// 用户列表
/// </summary>
public class UserItemDto
{
	public long Id { get; set; }

	/// <summary>
	/// 用户名
	/// </summary>
	public string Username { get; set; }

	/// <summary>
	/// 邮箱
	/// </summary>
	public string Email { get; set; }

	/// <summary>
	/// 电话
	/// </summary>
	public string Phone { get; set; }

	/// <summary>
	/// 全名
	/// </summary>
	public string Fullname { get; set; }

	/// <summary>
	/// 头像
	/// </summary>
	public string Avatar { get; set; }

	/// <summary>
	/// 锁定时间
	/// </summary>
	public DateTime? LockoutTime { get; set; }

	/// <summary>
	/// 访问失败次数
	/// </summary>
	public int AccessFailedCount { get; set; }

	/// <summary>
	/// 注册时间
	/// </summary>
	public DateTime CreateTime { get; set; }
}
