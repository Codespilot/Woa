using Postgrest.Attributes;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

[Table("refresh_tokens")]
public class RefreshTokenEntity : BaseModel, IEntity<int>
{
	[PrimaryKey("id")]
	public int Id { get; set; }

	[Column("token")]
	public string Token { get; set; }

	[Column("user_id")]
	public int UserId { get; set; }

	[Column("username")]
	public string Username { get; set; }

	[Column("expired_at")]
	public DateTime ExpiredAt { get; set; }

	[Column("is_valid")]
	public bool IsValid { get; set; }
}