namespace Woa.Webapi.Models;

/// <summary>
/// 微信消息类型
/// </summary>
public enum WechatMessageType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknown,

    /// <summary>
    /// 文本
    /// </summary>
    Text,

    /// <summary>
    /// 图片
    /// </summary>
    Image,

    /// <summary>
    /// 语音
    /// </summary>
    Voice,

    /// <summary>
    /// 音乐
    /// </summary>
    Music,

    /// <summary>
    /// 视频
    /// </summary>
    Video,

    /// <summary>
    /// 小视频
    /// </summary>
    /// <remarks>仅上行</remarks>
    ShortVideo,

    /// <summary>
    /// 地理位置
    /// </summary>
    /// <remarks>仅上行</remarks>
    Location,

    /// <summary>
    /// 链接
    /// </summary>
    /// <remarks>仅上行</remarks>
    Link,

    /// <summary>
    /// 图文
    /// </summary>
    News,

    /// <summary>
    /// 事件推送
    /// </summary>
    /// <remarks>仅上行</remarks>
    Event,
}