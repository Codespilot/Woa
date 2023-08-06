using Postgrest.Attributes;
using Postgrest.Models;

// ReSharper disable ExplicitCallerInfoArgument

namespace Woa.Webapi.Domain;

/// <summary>
/// 敏感词
/// </summary>
[Table("sensitive_words")]
public class SensitiveWord : BaseModel
{
    /// <summary>
    /// Id
    /// </summary>
    [PrimaryKey("id")]
    public int Id { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [Column("content")]
    public string Content { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    [Column("is_valid")]
    public bool IsValid { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("create_time")]
    public DateTime CreateTime { get; set; }
}