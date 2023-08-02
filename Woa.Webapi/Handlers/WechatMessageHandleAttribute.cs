using Woa.Webapi.Models;

namespace Woa.Webapi.Handlers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class WechatMessageHandleAttribute : Attribute
{
    public WechatMessageHandleAttribute(WechatMessageType type)
    {
        Type = type;
    }

    public WechatMessageType Type { get; }
}