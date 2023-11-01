namespace Woa.Transit;

/// <summary>
/// 角色信息
/// </summary>
public class RoleInfoDto
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 角色代码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateAt { get; set; }
}