﻿using Woa.Sdk.Tencent;

namespace Woa.Webapi.Wechat;

[WechatMessageHandle(WechatMessageType.Link)]
public class WechatLinkMessageHandler : WechatUserMessageHandler
{
	public WechatLinkMessageHandler(IWechatUserMessageStore store, WechatOptions options)
		: base(store, options)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, string platformId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		return await Task.FromResult(WechatMessage.Empty);
	}
}