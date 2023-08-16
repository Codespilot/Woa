namespace Woa.Sdk.Wechat;

public abstract class WechatUserMessageHandler : IWechatMessageHandler
{
	private readonly IWechatUserMessageStore _store;

	protected WechatUserMessageHandler(IWechatUserMessageStore store)
	{
		_store = store;
	}

	public Task<WechatMessage> HandleAsync(WechatMessage message, CancellationToken cancellationToken = default)
	{
		SaveMessage(message);
		var openId = message.GetValue<string>(WechatMessageKey.FromUserName);
		return HandleMessageAsync(openId, message, cancellationToken);
	}

	protected abstract Task<WechatMessage> HandleMessageAsync(string openId, WechatMessage message, CancellationToken cancellationToken = default);

	private async void SaveMessage(WechatMessage message)
	{
		if (_store != null)
		{
			await _store.SaveAsync(message);
		}
	}
}