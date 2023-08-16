namespace Woa.Sdk.Wechat;

public interface IWechatUserMessageStore
{
	Task SaveAsync(WechatMessage message);
}
