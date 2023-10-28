using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Woa.Webapi.Host;
using Woa.Webapi.Application;
using Woa.Webapi.Domain;
using Woa.Common;
using System.Collections;

namespace Woa.Webapi;

/// <summary>
/// 依赖注入扩展方法
/// </summary>
internal static class ServiceCollectionExtensions
{
	private static readonly List<Type> _types = typeof(Program).Assembly.GetTypes().ToList();

	/// <summary>
	/// 注册应用服务
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddTransient<LazyServiceProvider>();

		services.AddMediatR(config =>
				{
					config.Lifetime = ServiceLifetime.Scoped;
					config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
				})
				.AddObjectMapping()
				.AddObjectValidation();

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		var serviceTypes = _types.Where(type => typeof(IApplicationService).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
								 .ToList();

		foreach (var serviceType in serviceTypes)
		{
			services.AddScoped(serviceType);
			var interfaceTypes = serviceType.GetInterfaces()
											.Where(t => t != typeof(IApplicationService))
											.ToList();
			foreach (var interfaceType in interfaceTypes)
			{
				services.AddScoped(interfaceType, provider =>
				{
					var implementation = provider.GetService(serviceType);
					if (implementation is BaseApplicationService service)
					{
						service.ServiceProvider = provider.GetService<LazyServiceProvider>();
					}

					var properties = interfaceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
												  .Where(t => t.CanWrite && t.GetCustomAttribute<InjectAttribute>() != null);

					foreach (var property in properties)
					{
						if (property.PropertyType is { IsClass: false, IsInterface: false })
						{
							throw new InvalidOperationException("自动注入的属性类型必须是类或者接口");
						}

						if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))) //判断是否是获取多个实现实例
						{
							Type genericType;

							if (property.PropertyType.IsArray)
							{
								var baseInterfaceType = property.PropertyType.GetInterface("IEnumerable`1");

								if (baseInterfaceType == null)
								{
									throw new InvalidOperationException("类型错误");
								}

								if (baseInterfaceType.GenericTypeArguments.Length != 1)
								{
									throw new InvalidOperationException("仅允许一个泛型参数");
								}

								genericType = baseInterfaceType.GenericTypeArguments[0];
							}
							else
							{
								genericType = property.PropertyType.GenericTypeArguments[0];
							}

							if (!genericType.IsClass && !genericType.IsInterface)
							{
								throw new InvalidOperationException("自动注入的属性类型必须是类或者接口");
							}

							var values = provider.GetServices(genericType);

							var resultType = typeof(List<>).MakeGenericType(genericType);

							var resultInstance = Activator.CreateInstance(resultType);

							resultType!.GetMethod("AddRange")!.Invoke(resultInstance, new object[] { values });

							property.SetValue(implementation, resultInstance);
						}
						else
						{
							var value = provider.GetService(property.PropertyType);
							property.SetValue(implementation, value);
						}
					}

					return implementation;
				});
			}
		}

		return services;
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	/// 注册对象映射服务
	/// </summary>
	/// <param name="services"></param>
	/// <param name="config"></param>
	/// <returns></returns>
	internal static IServiceCollection AddObjectMapping(this IServiceCollection services, Action<MapperConfigurationExpression> config = null)
	{
		var expression = new MapperConfigurationExpression();

		if (_types != null)
		{
			foreach (var type in _types)
			{
				if (!typeof(Profile).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract)
				{
					continue;
				}

				expression.AddProfile(type);
			}
		}

		config?.Invoke(expression);
		var mapperConfiguration = new MapperConfiguration(expression);

		var mapper = mapperConfiguration.CreateMapper();

		services.AddSingleton(mapper);
		return services;
	}

	// ReSharper disable once MemberCanBePrivate.Global
	/// <summary>
	/// 注册对象验证服务
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	internal static IServiceCollection AddObjectValidation(this IServiceCollection services)
	{
		if (_types == null)
		{
			return services;
		}

		var implements = _types.Where(type => typeof(IValidator).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
							   .ToList();

		foreach (var validatorType in implements)
		{
			var inheritedType = validatorType.GetInterfaces().FirstOrDefault(t => t.IsGenericType);
			if (inheritedType == null)
			{
				continue;
			}

			if (inheritedType.GenericTypeArguments.Length != 1)
			{
				continue;
			}

			var objectType = inheritedType.GenericTypeArguments[0];
			if (!objectType.IsClass || objectType.IsAbstract || objectType.IsEnum)
			{
				continue;
			}

			var interfaceType = typeof(IValidator<>).MakeGenericType(objectType);
			services.AddSingleton(interfaceType, validatorType);
		}

		return services;
	}

	/// <summary>
	/// 注册领域服务
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	internal static IServiceCollection AddDomainServices(this IServiceCollection services)
	{
		services.AddScoped<IRepositoryContext, SupabaseRepositoryContext>();
		services.AddScoped(typeof(IRepository<,>), typeof(SupabaseRepository<,>));
		services.AddTransient<WechatFollowerRepository>()
				.AddTransient<WechatMessageRepository>()
				.AddTransient<WechatMenuRepository>()
				.AddTransient<UserRepository>()
				.AddTransient<RoleRepository>();
		return services;
	}

	internal static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
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

		return services;
	}

	internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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

		return services;
	}

	/// <summary>
	/// 注册背景任务服务
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
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
			// quartz.UseMicrosoftDependencyInjectionJobFactory();

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
}