using Postgrest.Attributes;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

/// <summary>
/// 用户角色配置
/// </summary>
[Table("user_role")]
public class UserRoleEntity : BaseModel, IEntity<int>
{
	[PrimaryKey("id")]
	public int Id { get; set; }

	/// <summary>
	/// 用户Id
	/// </summary>
	[Column("user_id")]
	public int UserId { get; set; }

	/// <summary>
	/// 角色Id
	/// </summary>
	[Column("role_id")]
	public int RoleId { get; set; }

	/// <summary>
	/// 创建时间
	/// </summary>
	[Column("create_at")]
	public DateTime CreateAt { get; set; }
}