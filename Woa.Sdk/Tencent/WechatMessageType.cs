using System.ComponentModel;

namespace Woa.Sdk.Tencent;

/// <summary>
/// 微信消息类型
/// </summary>
public enum WechatMessageType
{
    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    Unknown,

    /// <summary>
    /// 文本
    /// </summary>
    [Description("文本")]
    Text,

    /// <summary>
    /// 图片
    /// </summary>
    [Description("图片")]
    Image,

    /// <summary>
    /// 语音
    /// </summary>
    [Description("语音")]
    Voice,

    /// <summary>
    /// 音乐
    /// </summary>
    [Description("音乐")]
    Music,

    /// <summary>
    /// 视频
    /// </summary>
    [Description("视频")]
    Video,

    /// <summary>
    /// 小视频
    /// </summary>
    /// <remarks>仅上行</remarks>
    [Description("小视频")]
    ShortVideo,

    /// <summary>
    /// 地理位置
    /// </summary>
    /// <remarks>仅上行</remarks>
    [Description("地理位置")]
    Location,

    /// <summary>
    /// 链接
    /// </summary>
    /// <remarks>仅上行</remarks>
    [Description("链接")]
    Link,

    /// <summary>
    /// 图文
    /// </summary>
    [Description("图文")]
    News,

    /// <summary>
    /// 事件推送
    /// </summary>
    /// <remarks>仅上行</remarks>
    [Description("事件推送")]
    Event,
}