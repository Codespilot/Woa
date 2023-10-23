namespace Woa.Transit;

public class UserDetailDto
{
	public int Id { get; set; }

	public string Username { get; set; }

	public string Email { get; set; }

	public string Phone { get; set; }

	public string Fullname { get; set; }

	/// <summary>
    /// 简介
    /// </summary>
    public string Biography { get; set; }

	public string Avatar { get; set; }

	public DateTime? LockoutTime { get; set; }

	public List<string> Roles { get; set; }
}