namespace Woa.Sdk.Wechat;

public class WechatOptions
{
	public string Host { get; set; }

	public string AppId { get; set; }

	public string AppSecret { get; set; }

	public string AppToken { get; set; }

	public string EncodingKey { get; set; }

	public string EncryptType { get; set; }

	public string OpenId { get; set; }

	public bool EnableCustomMessage { get; set; }

	public bool EnableTemplateMessage { get; set; }

	public string ReplyTitle { get; set; }

	public string ReplyDescription { get; set; }

	public string ReplyUrl { get; set; }

	public string ReplyPicUrl { get; set; }
}
