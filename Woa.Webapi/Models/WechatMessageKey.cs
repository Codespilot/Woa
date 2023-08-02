namespace Woa.Webapi.Models;

/// <summary>
/// 微信公众号消息Xml节点Key
/// </summary>
internal static class WechatMessageKey
{
    /// <summary>
    /// 发送方账号
    /// </summary>
    public const string FromUserName = nameof(FromUserName);

    /// <summary>
    /// 接收方账号
    /// </summary>
    public const string ToUserName = nameof(ToUserName);

    /// <summary>
    /// 消息创建时间
    /// </summary>
    public const string CreateTime = nameof(CreateTime);

    /// <summary>
    /// 消息类型
    /// </summary>
    public const string MessageType = "MsgType";

    /// <summary>
    /// 事件消息
    /// </summary>
    public class Event
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public const string EventType = "Event";

        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public const string EventKey = nameof(EventKey);

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public const string Ticket = nameof(Ticket);

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public const string Latitude = nameof(Latitude);

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public const string Longitude = nameof(Longitude);

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public const string Precision = nameof(Precision);
    }

    /// <summary>
    /// 普通消息
    /// </summary>
    public class Standard
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public const string Content = nameof(Content);

        /// <summary>
        /// 消息id
        /// </summary>
        public const string MessageId = "MsgId";

        /// <summary>
        /// 消息的数据ID（消息如果来自文章时才有）
        /// </summary>
        public const string MessageDataId = "MsgDataId";

        /// <summary>
        /// 多图文时第几篇文章，从1开始（消息如果来自文章时才有）
        /// </summary>
        public const string MessageDataIds = "Idx";

        /// <summary>
        /// 图片链接（由系统生成）
        /// </summary>
        public const string PictureUrl = "PicUrl";

        /// <summary>
        /// 消息媒体id，可以调用获取临时素材接口拉取数据。
        /// 图片、语音、视频、小视频使用
        /// </summary>
        public const string MediaId = nameof(MediaId);

        /// <summary>
        /// 语音格式
        /// </summary>
        public const string Format = nameof(Format);

        /// <summary>
        /// 语音识别结果，UTF8编码
        /// </summary>
        public const string Recognition = nameof(Recognition);

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public const string ThumbMediaId = nameof(ThumbMediaId);

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public const string Latitude = "Location_X";

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public const string Longitude = "Location_Y";

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public const string Scale = nameof(Scale);

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public const string Label = nameof(Label);

        /// <summary>
        /// 消息标题
        /// </summary>
        public const string Title = nameof(Title);

        /// <summary>
        /// 消息描述
        /// </summary>
        public const string Description = nameof(Description);

        /// <summary>
        /// 消息链接
        /// </summary>
        public const string Url = nameof(Url);
    }

    /// <summary>
    /// 回复用户消息
    /// </summary>
    public class Reply
    {
        /// <summary>
        /// 回复的消息内容（换行：在content中能够换行，微信客户端就支持换行显示）
        /// </summary>
        public const string Content = nameof(Content);

        /// <summary>
        /// 图片消息
        /// </summary>
        public const string Image= nameof(Image);

        /// <summary>
        /// 语音消息
        /// </summary>
        public const string Voice = nameof(Voice);

        /// <summary>
        /// 视频消息
        /// </summary>
        public const string Video = nameof(Video);

        /// <summary>
        /// 音乐消息
        /// </summary>
        public const string Music = nameof(Music);

        /// <summary>
        /// 图文消息
        /// </summary>
        public const string Articles = nameof(Articles);

        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id。
        /// </summary>
        public const string MediaId = nameof(MediaId);

        /// <summary>
        /// 视频、音乐、图文消息标题
        /// </summary>
        public const string Title = nameof(Title);

        /// <summary>
        /// 视频、音乐消息的描述
        /// </summary>
        public const string Description = nameof(Description);

        /// <summary>
        /// 音乐链接
        /// </summary>
        public const string MusicUrl = "MusicURL";

        /// <summary>
        /// 高质量音乐链接，WIFI环境优先使用该链接播放音乐
        /// </summary>
        public const string HighQualityMusicUrl = "HQMusicUrl";

        /// <summary>
        /// 缩略图的媒体id，通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public const string ThumbMediaId = nameof(ThumbMediaId);

        /// <summary>
        /// 图文消息个数；当用户发送文本、图片、语音、视频、图文、地理位置这六种消息时，开发者只能回复1条图文消息；其余场景最多可回复8条图文消息
        /// </summary>
        public const string ArticleCount = nameof(ArticleCount);

        /// <summary>
        /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// </summary>
        public const string PicUrl = nameof(PicUrl);

        /// <summary>
        /// 点击图文消息跳转链接
        /// </summary>
        public const string Url = nameof(Url);
    }
}
