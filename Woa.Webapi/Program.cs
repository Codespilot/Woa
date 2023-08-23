using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Woa.Webapi;
using Woa.Webapi.Host;
using Woa.Sdk;
using Woa.Shared;
using Woa.Webapi.Wechat;

Log.Logger = new LoggerConfiguration().Enrich
                                      .FromLogContext()
                                      .WriteTo.Console()
                                      .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Debug)
                                                                      .WriteTo.File("Logs/Debug/logs.log", rollingInterval: RollingInterval.Day))
                                      .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Warning)
                                                                      .WriteTo.File("Logs/Warning/logs.log", rollingInterval: RollingInterval.Day))
                                      .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Error)
                                                                      .WriteTo.File("Logs/Error/logs.log", rollingInterval: RollingInterval.Day))
                                      .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Information)
                                                                      .WriteTo.File("Logs/Info/logs.log", rollingInterval: RollingInterval.Day))
                                      .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Fatal)
                                                                      .WriteTo.File("Logs/Fatal/logs.log", rollingInterval: RollingInterval.Day))
                                      .CreateLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseSerilog();
	builder.Services.AddTransient<ExceptionHandlingMiddleware>();
	builder.Services.AddAuthentication(builder.Configuration);
	builder.Services.AddControllers();

	builder.Services.AddApplicationServices().AddDomainServices();

	builder.Services.AddMemoryCache();

	builder.Services.AddSupabaseClient(builder.Configuration);

	builder.Services
	       .AddWechatApi(builder.Configuration.GetSection("Wechat"))
	       .AddWechatMessageHandler<WechatUserMessageStore>(typeof(Program).Assembly);

	builder.Services.AddHostedService<ChatbotBackgroundService>();
	builder.Services.AddRecurringJobService();

	builder.Services.AddSwaggerGen(gen =>
	{
		gen.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.Http,
			Scheme = "bearer"
		});

		gen.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				},
				new List<string>()
			}
		});

		gen.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "API",
			Version = "v1",
			License = new OpenApiLicense
			{
				Name = "© 2020 Nerosoft. All Rights Reserved."
			}
		});
	});

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
	}

	app.UseSwagger();
	app.UseSwaggerUI(option =>
	{
		option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1.0");
	});

	app.UseAuthentication();
	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (Exception exception)
{
	Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}