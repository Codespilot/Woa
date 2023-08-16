using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Woa.Webapi.Jobs;
using Woa.Webapi.Application;
using Supabase;

namespace Woa.Webapi;

internal static class ServiceCollectionExtensions
{
	internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddMediatR(config =>
		{
			config.Lifetime = ServiceLifetime.Scoped;
			config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});
		return services.AddTransient<IUserService, UserService>();
	}

	internal static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

		var bearerOptions = configuration.GetSection(nameof(JwtBearerOptions)).Get<JwtBearerOptions>();

		var tokenKey = configuration.GetValue<string>("JwtBearerOptions:TokenKey");
		var key = Encoding.UTF8.GetBytes(tokenKey.ToSha256());
		var issuer = configuration.GetValue<string>("JwtBearerOptions:TokenIssuer");

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.Authority = bearerOptions.Authority;
			options.RequireHttpsMetadata = bearerOptions.RequireHttpsMetadata;
			options.Audience = bearerOptions.Audience;

			options.Events = new JwtBearerEvents()
			{
				OnMessageReceived = context =>
				{
					context.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
					return Task.CompletedTask;
				},
				OnChallenge = _ => Task.CompletedTask
			};
			options.TokenValidationParameters = new TokenValidationParameters
			{
				NameClaimType = JwtClaimTypes.Name,
				RoleClaimType = JwtClaimTypes.Role,
				ValidIssuers = new[] { issuer },
				//ValidAudience = "api",
				ValidateIssuer = true,
				ValidateAudience = false,
				IssuerSigningKey = new SymmetricSecurityKey(key)
			};
		});
	}

	internal static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		var bearerOptions = configuration.GetSection(nameof(JwtBearerOptions)).Get<JwtBearerOptions>();
		var issuer = configuration.GetValue<string>("JwtBearerOptions:TokenIssuer");
		var tokenKey = configuration.GetValue<string>("JwtBearerOptions:TokenKey");
		var key = Encoding.UTF8.GetBytes(tokenKey.ToSha256());

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuerSigningKey = true,
						ValidIssuer = issuer,
						ValidAudience = bearerOptions.Audience,
						//用于签名验证
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				});
	}

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

	// ReSharper disable once MemberCanBePrivate.Global
	internal static IServiceCollection AddSupabaseClient(this IServiceCollection services, string url, string key)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			throw new NullReferenceException("Supabase:Url is null or empty");
		}

		if (string.IsNullOrWhiteSpace(key))
		{
			throw new NullReferenceException("Supabase:Key is null or empty");
		}

		var options = new SupabaseOptions
		{
			AutoRefreshToken = true,
			AutoConnectRealtime = true
		};
		services.AddSingleton(_ => new SupabaseClient(url, key, options));

		return services;
	}

	internal static IServiceCollection AddSupabaseClient(this IServiceCollection services, IConfiguration configuration)
	{
		var url = configuration["Supabase:Url"];
		var key = configuration["Supabase:Key"];
		if (string.IsNullOrWhiteSpace(url))
		{
			throw new NullReferenceException("Supabase:Url is null or empty");
		}

		if (string.IsNullOrWhiteSpace(key))
		{
			throw new NullReferenceException("Supabase:Key is null or empty");
		}

		return services.AddSupabaseClient(url, key);
	}

	internal static IServiceCollection AddSupabaseClient(this IServiceCollection services)
	{
		var options = new SupabaseOptions
		{
			AutoRefreshToken = true,
			AutoConnectRealtime = true
		};
		return services.AddSingleton(provider =>
		{
			var configuration = provider.GetRequiredService<IConfiguration>();
			var url = configuration["Supabase:Url"];
			var key = configuration["Supabase:Key"];
			if (string.IsNullOrWhiteSpace(url))
			{
				throw new NullReferenceException("Supabase:Url is null or empty");
			}

			if (string.IsNullOrWhiteSpace(key))
			{
				throw new NullReferenceException("Supabase:Key is null or empty");
			}

			return new SupabaseClient(url, key, options);
		});
	}
}