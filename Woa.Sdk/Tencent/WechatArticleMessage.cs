﻿namespace Woa.Sdk.Tencent;

/// <summary>
/// 微信图文消息
/// </summary>
public class WechatArticleMessage
{
    /// <summary>
    /// 图文消息标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 图文消息描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
    /// </summary>
    public string PicUrl { get; set; }

    /// <summary>
    /// 点击图文消息跳转链接
    /// </summary>
    public string Url { get; set; }
}
