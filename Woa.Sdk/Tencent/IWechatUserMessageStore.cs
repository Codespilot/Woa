namespace Woa.Sdk.Tencent;

public interface IWechatUserMessageStore
{
	/// <summary>
	/// 保存消息
	/// </summary>
	/// <param name="message">微信用户消息</param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task SaveAsync(WechatMessage message, CancellationToken cancellationToken = default);
}