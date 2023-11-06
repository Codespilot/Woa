namespace Woa.Sdk.Tencent;

/// <summary>
/// 微信消息处理接口
/// </summary>
public interface IWechatMessageHandler
{
	/// <summary>
	/// 接收并处理指定类型的微信消息
	/// </summary>
	/// <param name="message">接收到的微信消息对象</param>
	/// <param name="payload">微信消息报文</param>
	/// <param name="cancellationToken"></param>
	/// <returns>需要回复用户的消息内容</returns>
	Task<WechatMessage> HandleAsync(WechatMessage message, string payload, CancellationToken cancellationToken = default);
}