using Postgrest.Attributes;

namespace Woa.Webapi.Domain;

[Table("user_role")]
public class UserRoleEntity : IEntity<int>
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }
}