using Postgrest.Attributes;
using Postgrest.Models;

namespace Woa.Webapi.Domain;

/// <summary>
/// 角色实体
/// </summary>
[Table("roles")]
public class RoleEntity : BaseModel, 
						  IEntity<int>,
						  ICreateAudit<int>,
						  IUpdateAudit<int?>
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

	/// <summary>
	/// 描述
	/// </summary>
	[Column("description")]
	public string Description { get; set; }

	[Column("create_by", ignoreOnUpdate: true)]
	public int CreateBy { get; set; }

	[Column("create_at", ignoreOnUpdate: true)]
	public DateTime CreateAt { get; set; }

	[Column("update_by", ignoreOnInsert: true)]
	public int? UpdateBy { get; set; }

	[Column("update_at", ignoreOnInsert: true)]
	public DateTime? UpdateAt { get; set; }

	object ICreateAudit.CreateBy
	{
		get => CreateBy;
		set => CreateBy = value switch
		{
			int i => i,
			_ => throw new ArgumentException("CreateBy must be int")
		};
	}

	object IUpdateAudit.UpdateBy
	{
		get => UpdateBy;
		set => UpdateBy = value switch
		{
			null => null,
			int i => i,
			_ => throw new ArgumentException("UpdateBy must be int or null")
		};
	}
}