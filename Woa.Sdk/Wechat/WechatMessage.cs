using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;

namespace Woa.Sdk.Wechat;

/// <summary>
/// 微信消息对象
/// </summary>
public class WechatMessage : Dictionary<string, object>
{
    private readonly string _xml;

    public WechatMessage()
    {
    }

    private WechatMessage(string xml)
    {
        _xml = xml;
    }

    public WechatMessage(WechatMessageType type)
    {
        MessageType = type;
    }

    /// <summary>
    /// 消息id，64位整型
    /// </summary>
    /// <remarks>
    /// XML报文中为MsgId
    /// </remarks>
    public long MessageId
    {
        get
        {
            if (!TryGetValue(WechatMessageKey.Standard.MessageId, out var value))
            {
                return 0;
            }

            return long.TryParse(value.ToString(), out var result) ? result : 0;
        }
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    /// <value>
    /// 参见<see cref="WechatMessageType"/>
    /// </value>
    /// <remarks>
    /// XML报文中为MsgType，需要转换为小写
    /// </remarks>
    public WechatMessageType MessageType
    {
        get => !TryGetValue<string>(WechatMessageKey.MessageType, out var result) ? WechatMessageType.Unknown : Enum.Parse<WechatMessageType>(result, true);
        private set => this[WechatMessageKey.MessageType] = value.ToString().ToLower(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// 消息创建时间
    /// </summary>
    /// <remarks>
    /// XML报文中为整型的UNIX时间戳，需要转换为DateTime
    /// </remarks>
    public DateTime CreateTime
    {
        get
        {
            if (!TryGetValue(nameof(CreateTime), out var value))
            {
                return DateTime.MinValue;
            }

            return long.TryParse(value.ToString(), out var time) ? DateTimeOffset.FromUnixTimeSeconds(time).DateTime : DateTime.MinValue;
        }
        set => this[WechatMessageKey.CreateTime] = new DateTimeOffset(value).ToUnixTimeSeconds();
    }

    internal T GetValue<T>(string key)
    {
        if (!TryGetValue(key, out var value))
        {
            return default;
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }

    internal T GetValue<T>(string key, Func<object, T> func)
    {
        return !TryGetValue(key, out var value) ? default : func(value);
    }

    internal bool TryGetValue<T>(string key, out T value)
    {
        if (!TryGetValue(key, out var result))
        {
            value = default;
            return false;
        }

        if (result is T t)
        {
            value = t;
            return true;
        }

        try
        {
            value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(result);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// 从Xml内容解析微信消息对象
    /// </summary>
    /// <param name="xml">XML包内容</param>
    /// <returns></returns>
    internal static WechatMessage Parse(string xml)
    {
        var doc = XDocument.Parse(xml);
        var message = new WechatMessage(xml);

        foreach (var element in doc.Descendants().Where(p => p.HasElements == false))
        {
            var keyIndex = 0;
            var keyName = element.Name.LocalName;

            while (message.ContainsKey(keyName))
            {
                keyName = element.Name.LocalName + "_" + keyIndex++;
            }

            message.Add(keyName, element.Value);
        }

        return message;
    }

    /// <summary>
    /// 将微信消息对象转换为Xml内容
    /// </summary>
    /// <returns></returns>
    internal string ToXml()
    {
        var xml = new XElement("xml");

        foreach (var (key, value) in this)
        {
            var child = GetXml(key, value);
            xml.Add(child);
        }

        var outXml = xml.ToString();
        return outXml;
    }

    internal static WechatMessage Empty => null;

    private static XElement GetXml(string name, object value)
    {
        switch (value)
        {
            case null:
                return new XElement(name);
            case string stringValue:
                return new XElement(name, new XCData(stringValue));
            case int:
            case long:
            case double:
            case decimal:
            case float:
            case bool:
            case DateTime:
            case TimeSpan:
            case char:
                return new XElement(name, value);
            case IDictionary dictionary:
            {
                var xml = new XElement(name);
                foreach (var key in dictionary.Keys)
                {
                    xml.Add(GetXml(key.ToString(), dictionary[key]));
                }

                return xml;
            }
            case IEnumerable list:
            {
                var element = new XElement(name);
                foreach (var item in list)
                {
                    var child = GetXml("item", item);
                    element.Add(child);
                }

                return element;
            }
            default:
            {
                var type = value.GetType();
                if (type.IsEnum)
                {
                    return new XElement(name, value);
                }

                var element = new XElement(name);
                var properties = type.GetRuntimeProperties();
                foreach (var property in properties)
                {
                    var child = GetXml(property.Name, property.GetValue(value));
                    element.Add(child);
                }

                return element;
            }
        }
    }

    internal string GetOriginXml()
    {
        return _xml;
    }
}