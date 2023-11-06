namespace Woa.Sdk.Tencent;

public interface IWechatUserMessageStore
{
	/// <summary>
	/// 保存消息
	/// </summary>
	/// <param name="message">微信用户消息</param>
	/// <param name="payload">微信消息报文</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task SaveAsync(WechatMessage message, string payload, CancellationToken cancellationToken = default);
}