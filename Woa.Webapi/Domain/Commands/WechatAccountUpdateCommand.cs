﻿namespace Woa.Webapi.Domain;

public class WechatAccountUpdateCommand : ICommand
{
	public WechatAccountUpdateCommand(string id)
	{
		Id = id;
	}

	public string Id { get; }

	/// <summary>
	/// 微信号
	/// </summary>
	public string Account { get; set; }

	/// <summary>
	/// 公众号名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 公众号简介
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 公众号头像
	/// </summary>
	public string Image { get; set; }

	/// <summary>
	/// 开发者密码
	/// </summary>
	public string AppSecret { get; set; }

	/// <summary>
	/// 开发者令牌
	/// </summary>
	/// <remarks>
	/// 必须为英文或数字，长度为3-32字符。
	/// </remarks>
	public string AppToken { get; set; }

	/// <summary>
	/// 消息加解密密钥
	/// </summary>
	/// <remarks>
	/// 消息加密密钥由43位字符组成，可随机修改，字符范围为A-Z，a-z，0-9。
	/// </remarks>
	public string EncryptKey { get; set; }

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
	/// 公众号消息回复内容标题
	/// </summary>
	public string ReplyTitle { get; set; }

	/// <summary>
	/// 公众号消息回复内容描述
	/// </summary>
	public string ReplyDescription { get; set; }

	/// <summary>
	/// 公众号消息回复内容查看链接
	/// </summary>
	public string ReplyUrl { get; set; }

	/// <summary>
	/// 公众号消息回复图片链接
	/// </summary>
	public string ReplyPicUrl { get; set; }
}