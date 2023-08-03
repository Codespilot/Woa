using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Quartz;
using Supabase;
using Woa.Common;
using Woa.Webapi.Handlers;
using Woa.Webapi.Jobs;

namespace Woa.Webapi;

internal static class ServiceCollectionExtensions
{
    private static readonly Dictionary<string, Type> _handlerTypes = GetWechatMessageHandlers();

    internal static IServiceCollection AddRecurringJobService(this IServiceCollection services)
    {
        services.AddTransient<WechatAccessTokenGrantJob>();

        // if you are using persistent job store, you might want to alter some options
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });

        services.AddQuartz(quartz =>
        {
            // handy when part of cluster or you want to otherwise identify multiple schedulers
            quartz.SchedulerId = "Scheduler-Core";

            // we take this from appsettings.json, just show it's possible
            // q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

            // as of 3.3.2 this also injects scoped services (like EF DbContext) without problems
            quartz.UseMicrosoftDependencyInjectionJobFactory();

            // or for scoped service support like EF Core DbContext
            // q.UseMicrosoftDependencyInjectionScopedJobFactory();

            // these are the defaults
            quartz.UseSimpleTypeLoader();
            quartz.UseInMemoryStore();
            quartz.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 10;
            });

            // quickest way to create a job with single trigger is to use ScheduleJob
            // (requires version 3.2)
            quartz.ScheduleJob<WechatAccessTokenGrantJob>(trigger => trigger.WithIdentity("WechatAccessTokenGrantJob")
                                                                            .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                                                                            .WithSimpleSchedule(x => x.WithIntervalInMinutes(30).RepeatForever())
                //.WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                //.WithDescription("my awesome trigger configured for a job with single call")
            );

            /*
            // you can also configure individual jobs and triggers with code
            // this allows you to associated multiple triggers with same job
            // (if you want to have different job data map per trigger for example)
            quartz.AddJob<WechatAccessTokenGrantJob>(j => j
                                                          .StoreDurably() // we need to store durably if no trigger is associated
                                                          .WithDescription("my awesome job")
            );

            // here's a known job for triggers
            var jobKey = new JobKey("awesome job", "awesome group");
            quartz.AddJob<WechatAccessTokenGrantJob>(jobKey, j => j
                .WithDescription("my awesome job")
            );

            quartz.AddTrigger(t => t
                                   .WithIdentity("Simple Trigger")
                                   .ForJob(jobKey)
                                   .StartNow()
                                   .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever())
                                   .WithDescription("my awesome simple trigger")
            );

            quartz.AddTrigger(t => t
                                   .WithIdentity("Cron Trigger")
                                   .ForJob(jobKey)
                                   .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(3)))
                                   .WithCronSchedule("0/3 * * * * ?")
                                   .WithDescription("my awesome cron trigger")
            );

            // you can add calendars too (requires version 3.2)
            const string calendarName = "myHolidayCalendar";
            quartz.AddCalendar<HolidayCalendar>(
                name: calendarName,
                replace: true,
                updateTriggers: true,
                x => x.AddExcludedDate(new DateTime(2020, 5, 15))
            );

            quartz.AddTrigger(t => t
                                   .WithIdentity("Daily Trigger")
                                   .ForJob(jobKey)
                                   .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(5)))
                                   .WithDailyTimeIntervalSchedule(x => x.WithInterval(10, IntervalUnit.Second))
                                   .WithDescription("my awesome daily time interval trigger")
                                   .ModifiedByCalendar(calendarName)
            );

            // also add XML configuration and poll it for changes
            quartz.UseXmlSchedulingConfiguration(x =>
            {
                x.Files = new[] { "~/quartz_jobs.config" };
                x.ScanInterval = TimeSpan.FromSeconds(2);
                x.FailOnFileNotFound = true;
                x.FailOnSchedulingError = true;
            });

            // convert time zones using converter that can handle Windows/Linux differences
            quartz.UseTimeZoneConverter();

            // auto-interrupt long-running job
            quartz.UseJobAutoInterrupt(options =>
            {
                // this is the default
                options.DefaultMaxRunTime = TimeSpan.FromMinutes(5);
            });
            quartz.ScheduleJob<SlowJob>(
                triggerConfigurator => triggerConfigurator
                                       .WithIdentity("slowJobTrigger")
                                       .StartNow()
                                       .WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever()),
                jobConfigurator => jobConfigurator
                                   .WithIdentity("slowJob")
                                   .UsingJobData(JobInterruptMonitorPlugin.JobDataMapKeyAutoInterruptable, true)
                                   // allow only five seconds for this job, overriding default configuration
                                   .UsingJobData(JobInterruptMonitorPlugin.JobDataMapKeyMaxRunTime, TimeSpan.FromSeconds(5).TotalMilliseconds.ToString(CultureInfo.InvariantCulture)));

            // add some listeners
            quartz.AddSchedulerListener<SampleSchedulerListener>();
            quartz.AddJobListener<SampleJobListener>(GroupMatcher<JobKey>.GroupEquals(jobKey.Group));
            quartz.AddTriggerListener<SampleTriggerListener>();

            // example of persistent job store using JSON serializer as an example
            
            quartz.UsePersistentStore(s =>
            {
                s.PerformSchemaValidation = true; // default
                s.UseProperties = true; // preferred, but not default
                s.RetryInterval = TimeSpan.FromSeconds(15);
                s.UseSqlServer(sqlServer =>
                {
                    sqlServer.ConnectionString = "some connection string";
                    // this is the default
                    sqlServer.TablePrefix = "QRTZ_";
                });
                s.UseJsonSerializer();
                s.UseClustering(c =>
                {
                    c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                    c.CheckinInterval = TimeSpan.FromSeconds(10);
                });
            });
            //*/
        });

        services.AddQuartzHostedService(options =>
        {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });

        // services.AddTransient<IJobFactory, DefaultJobFactory>();
        // services.AddHostedService<RecurringJobService>();
        return services;
    }

    
    internal static IServiceCollection AddWechatMessageHandler(this IServiceCollection services)
    {
        foreach (var (_, type) in _handlerTypes)
        {
            services.AddScoped(type);
        }

        services.AddTransient<NamedService<IWechatMessageHandler>>(provider =>
        {
            return name =>
            {
                if (_handlerTypes.TryGetValue(name, out var type))
                {
                    return provider.GetRequiredService(type) as IWechatMessageHandler;
                }

                return default;
            };
        });

        return services;
    }

    private static Dictionary<string, Type> GetWechatMessageHandlers()
    {
        var types = typeof(IWechatMessageHandler).Assembly
                                                 .GetTypes()
                                                 .Where(type => !type.IsAbstract && typeof(IWechatMessageHandler).IsAssignableFrom(type));
        var handlers = new Dictionary<string, Type>();
        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes<WechatMessageHandleAttribute>();
            if (attributes.Any())
            {
                foreach (var attribute in attributes)
                {
                    handlers.Add(attribute.Type.ToString(), type);
                }
            }
            else
            {
                var match = Regex.Match(type.Name, @"^Wechat(\w+)MessageHandler$");

                if (match.Success)
                {
                    handlers.Add(match.Groups[1].Value.ToLower(CultureInfo.CurrentCulture), type);
                }
            }
        }

        return handlers;
    }
}