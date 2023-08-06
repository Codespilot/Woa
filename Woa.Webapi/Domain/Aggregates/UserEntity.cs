using Postgrest.Attributes;
using Postgrest.Models;

// ReSharper disable ExplicitCallerInfoArgument

namespace Woa.Webapi.Domain;

[Table("users")]
public class UserEntity : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("username")]
    public string Username { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Column("password_salt")]
    public string PasswordSalt { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("phone")]
    public string Phone { get; set; }

    [Column("fullname")]
    public string Fullname { get; set; }

    [Column("avatar")]
    public string Avatar { get; set; }

    [Column("lockout_time")]
    public DateTime? LockoutTime { get; set; }

    [Column("access_failed_count")]
    public int AccessFailedCount { get; set; }

    [Column("create_time")]
    public DateTime CreateTime { get; set; }

    [Column("update_time")]
    public DateTime? UpdateTime { get; set; }

    [Column("delete_time")]
    public DateTime? DeleteTime { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}