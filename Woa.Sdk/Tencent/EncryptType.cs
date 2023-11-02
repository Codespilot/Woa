using System.ComponentModel;

namespace Woa.Sdk.Tencent;

/// <summary>
/// 消息加解密方式
/// </summary>
public enum EncryptType
{
    /// <summary>
    /// 明文模式
    /// </summary>
    /// <remarks>
    /// 不使用消息体加解密功能，安全系数较低
    /// </remarks>
    [Description("明文模式")]
    None,

    /// <summary>
    /// 兼容模式
    /// </summary>
    /// <remarks>
    /// 明文、密文将共存，方便开发者调试和维护
    /// </remarks>
    [Description("兼容模式")]
    Compatible,

    /// <summary>
    /// 安全模式
    /// </summary>
    /// <remarks>
    /// 消息包为纯密文，需要开发者加密和解密，安全系数高
    /// </remarks>
    [Description("安全模式")]
    Safety,
}