namespace Woa.Transit;

/// <summary>
/// 用户更新数据传输对象
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string Fullname { get; set; }

    /// <summary>
    /// 简介
    /// </summary>
    public string Biography { get; set; }
}