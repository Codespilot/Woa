namespace Woa.Transit;

public class UserDetailDto
{
	public int Id { get; set; }

	public string Username { get; set; }

	public string Email { get; set; }

	public string Phone { get; set; }

	public string Fullname { get; set; }

	public string Avatar { get; set; }

	public DateTime? LockoutTime { get; set; }

	public List<string> Roles { get; set; }
}