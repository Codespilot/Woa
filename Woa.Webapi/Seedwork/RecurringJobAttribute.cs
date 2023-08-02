namespace Woa.Webapi;

[AttributeUsage(AttributeTargets.Class)]
public class RecurringJobAttribute : Attribute
{
    public RecurringJobAttribute(string identity)
    {
        Identity = identity;
    }

    /// <summary>
    /// 任务唯一标记
    /// </summary>
    public string Identity { get; }

    /// <summary>
    /// 间隔时间（分钟）
    /// </summary>
    public int Interval { get; set; }

    /// <summary>
    /// 表达式
    /// </summary>
    public string Cron { get; set; }

    /// <summary>
    /// 启动延迟（秒）
    /// </summary>
    public int DelaySeconds { get; set; } = 5;
}