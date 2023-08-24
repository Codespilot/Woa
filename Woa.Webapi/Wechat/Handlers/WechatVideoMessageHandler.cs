using Woa.Sdk.Wechat;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 微信用户视频消息处理器
/// </summary>
[WechatMessageHandle(WechatMessageType.Video), WechatMessageHandle(WechatMessageType.ShortVideo)]
public class WechatVideoMessageHandler : WechatUserMessageHandler
{
	public WechatVideoMessageHandler(IWechatUserMessageStore store)
		: base(store)
	{
	}

	/// <inheritdoc />
	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		return await Task.FromResult(WechatMessage.Empty);
	}
}