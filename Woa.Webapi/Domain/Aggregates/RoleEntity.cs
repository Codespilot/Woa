using Postgrest.Attributes;

namespace Woa.Webapi.Domain;

/// <summary>
/// 角色实体
/// </summary>
[Table("roles")]
public class RoleEntity : IEntity<int>
{
	[PrimaryKey("id")]
	public int Id { get; set; }

	/// <summary>
	/// 角色代码
	/// </summary>
	[Column("code")]
	public string Code { get; set; }

	/// <summary>
	/// 角色名称
	/// </summary>
	[Column("name")]
	public string Name { get; set; }
}
