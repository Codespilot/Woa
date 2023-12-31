﻿using Woa.Sdk.Tencent;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 
/// </summary>
[WechatMessageHandle(WechatMessageType.Location)]
public class WechatLocationMessageHandler : WechatUserMessageHandler
{
	public WechatLocationMessageHandler(IWechatUserMessageStore store, WechatOptions options)
		: base(store, options)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, string platformId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		var content = $"LAT:{message.GetValue<double>(WechatMessageKey.Standard.Latitude)} LNG:{message.GetValue<double>(WechatMessageKey.Standard.Longitude)}";
		var reply = WechatMessage.Text(content);
		return await Task.FromResult(reply);
	}
}