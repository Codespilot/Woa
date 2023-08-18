namespace Woa.Sdk.Wechat;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class WechatMessageHandleAttribute : Attribute
{
    public WechatMessageHandleAttribute(WechatMessageType type)
    {
        Type = type;
    }

    public WechatMessageType Type { get; }
}