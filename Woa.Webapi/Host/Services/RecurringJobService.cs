using System.Reflection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Woa.Webapi.Host;

public class RecurringJobService : BackgroundService
{
    private readonly IScheduler _scheduler;

    public RecurringJobService(IJobFactory jobFactory)
    {
        var factory = new StdSchedulerFactory();
        _scheduler = factory.GetScheduler().Result; //AsyncHelper.RunSync(async () => await factory.GetScheduler());
        _scheduler.JobFactory = jobFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _scheduler.Start(stoppingToken);

        var types = new List<Type>
        {
            typeof(WechatAccessTokenGrantJob)
        };

        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute<RecurringJobAttribute>();

            if (attribute == null)
            {
                continue;
            }

            if (attribute.Interval == 0 && string.IsNullOrEmpty(attribute.Cron))
            {
                continue;
            }

            var job = JobBuilder.Create(type)
                                .Build();
            var trigger = TriggerBuilder.Create()
                                        .WithIdentity(attribute.Identity, "RecurringJobTriggerGroup")
                                        .WithSimpleSchedule(x => x.WithIntervalInMinutes(attribute.Interval).RepeatForever())
                                        .StartAt(new DateTimeOffset(DateTime.UtcNow.AddSeconds(attribute.DelaySeconds)))
                                        .Build();
            await _scheduler.ScheduleJob(job, trigger, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scheduler.Shutdown(cancellationToken);
        await _scheduler.Clear(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}