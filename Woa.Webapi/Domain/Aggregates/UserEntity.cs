using Postgrest.Attributes;
using Postgrest.Models;

// ReSharper disable ExplicitCallerInfoArgument

namespace Woa.Webapi.Domain;

[Table("users")]
public class UserEntity : BaseModel, IEntity<int>
{
    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    [PrimaryKey("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    [Column("username")]
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password hash.
    /// </summary>
    [Column("password_hash")]
    public string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the password salt.
    /// </summary>
    [Column("password_salt")]
    public string PasswordSalt { get; set; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    [Column("email")]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    [Column("phone")]
    public string Phone { get; set; }

    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    [Column("fullname")]
    public string Fullname { get; set; }

    /// <summary>
    /// Gets or sets the avatar URL.
    /// </summary>
    [Column("avatar")]
    public string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the lockout time.
    /// </summary>
    [Column("lockout_time")]
    public DateTime? LockoutTime { get; set; }

    /// <summary>
    /// Gets or sets the access failed count.
    /// </summary>
    [Column("access_failed_count")]
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Gets or sets the create time.
    /// </summary>
    [Column("create_time")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// Gets or sets the update time.
    /// </summary>
    [Column("update_time")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// Gets or sets the delete time.
    /// </summary>
    [Column("delete_time")]
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this user is deleted.
    /// </summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}
