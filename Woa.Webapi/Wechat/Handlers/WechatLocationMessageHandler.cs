using Woa.Sdk.Wechat;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 
/// </summary>
[WechatMessageHandle(WechatMessageType.Location)]
public class WechatLocationMessageHandler : WechatUserMessageHandler
{
	public WechatLocationMessageHandler(IWechatUserMessageStore store)
		: base(store)
	{
	}

	protected override async Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		var content = $"LAT:{message.GetValue<double>(WechatMessageKey.Standard.Latitude)} LNG:{message.GetValue<double>(WechatMessageKey.Standard.Longitude)}";
		var reply = WechatMessage.Text(content);
		return await Task.FromResult(reply);
	}
}