﻿namespace Woa.Sdk.Tencent;

/// <summary>
/// 微信用户消息处理接口
/// </summary>
public abstract class WechatUserMessageHandler : IWechatMessageHandler
{
	private readonly IWechatUserMessageStore _store;

	/// <summary>
	/// 初始化一个 <see cref="WechatUserMessageHandler"/> 对象
	/// </summary>
	/// <param name="store">微信消息保存服务</param>
	protected WechatUserMessageHandler(IWechatUserMessageStore store)
	{
		_store = store;
	}

	/// <inheritdoc />
	public Task<WechatMessage> HandleAsync(WechatMessage message, CancellationToken cancellationToken = default)
	{
		SaveMessage(message);
		var openId = message.GetValue<string>(WechatMessageKey.FromUserName);
		var platformId = message.GetValue<string>(WechatMessageKey.ToUserName);
		return HandleMessageAsync(openId, platformId, message, cancellationToken);
	}

	/// <summary>
	/// 处理指定用户的微信消息
	/// </summary>
	/// <param name="openId"></param>
	/// <param name="platformId"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	/// <param name="cancellationToken"></param>
	protected abstract Task<WechatMessage> HandleMessageAsync(string openId, string platformId, WechatMessage message, CancellationToken cancellationToken = default);

	/// <summary>
	/// 保存微信消息
	/// </summary>
	/// <param name="message"></param>
	private async void SaveMessage(WechatMessage message)
	{
		if (_store != null)
		{
			await _store.SaveAsync(message);
		}
	}
}