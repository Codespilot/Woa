namespace Woa.Sdk.Tencent;

public class WechatOptions
{
	/// <summary>
	/// 微信主机地址
	/// </summary>
	public string Host { get; set; }

	/// <summary>
	/// 开发者令牌
	/// </summary>
	public string Token { get; set; }

	/// <summary>
	/// 微信公众号配置
	/// </summary>
	// ReSharper disable once CollectionNeverUpdated.Global
	public Dictionary<string, WechatPlatformAccount> Accounts { get; set; }

	public string ReplyTitle { get; set; }

	public string ReplyDescription { get; set; }

	public string ReplyUrl { get; set; }

	public string ReplyPicUrl { get; set; }
}

public class WechatPlatformAccount
{
	/// <summary>
	/// 公众号名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 开发者ID
	/// </summary>
	public string AppId { get; set; }

	/// <summary>
	/// 开发者密码
	/// </summary>
	public string AppSecret { get; set; }

	/// <summary>
	/// 消息加解密密钥
	/// </summary>
	public string EncryptKey { get; set; }

	/// <summary>
	/// 消息加解密方式
	/// </summary>
	public EncryptType EncryptType { get; set; }

	/// <summary>
	/// 公众号是否开启客服消息
	/// </summary>
	public bool EnableCustomMessage { get; set; }

	/// <summary>
	/// 公众号是否开启模板消息
	/// </summary>
	public bool EnableTemplateMessage { get; set; }

	/// <summary>
	/// 是否启用
	/// </summary>
	public bool Enabled { get; set; }
}