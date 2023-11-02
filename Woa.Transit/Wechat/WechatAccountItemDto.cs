﻿namespace Woa.Transit;

public class WechatAccountItemDto
{
	/// <summary>
	/// 微信公众号Id（OpenId）
	/// </summary>
	public string Id { get; set; }

	/// <summary>
	/// 微信号
	/// </summary>
	public string Account { get; set; }

	/// <summary>
	/// 类型
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// 类型名称
	/// </summary>
	public string TypeName { get; set; }

	/// <summary>
	/// 公众号名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 开发者ID
	/// </summary>
	public string AppId { get; set; }

	/// <summary>
	/// 公众号AppSecret
	/// </summary>
	public string AppSecret { get; set; }

	/// <summary>
	/// 公众号消息加密方式
	/// </summary>
	public string EncryptType { get; set; }

	/// <summary>
	/// 公众号是否开启客服消息
	/// </summary>
	public bool EnableCustomMessage { get; set; }

	/// <summary>
	/// 公众号是否开启模板消息
	/// </summary>
	public bool EnableTemplateMessage { get; set; }

	/// <summary>
	/// 是否已认证
	/// </summary>
	public bool Verified { get; set; }
	
	/// <summary>
	/// 是否有效
	/// </summary>
	public bool IsValid { get; set; }

	public DateTime CreateAt { get; set; }

	public DateTime? UpdateAt { get; set; }

	public string CreateBy { get; set; }

	public string UpdateBy { get; set; }
}