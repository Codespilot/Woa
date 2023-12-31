﻿using Microsoft.OpenApi.Models;
using Woa.Sdk;
using Woa.Shared;
using Woa.Webapi.Host;
using Woa.Webapi.Wechat;

namespace Woa.Webapi;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddTransient<ExceptionHandlingMiddleware>();

		services.AddHttpContextAccessor()
		        .AddScoped<IIdentityContext, IdentityContext>();

		services.AddAuthentication(Configuration);

		services.AddApplicationServices()
		        .AddDomainServices();

		services.AddControllers().AddNewtonsoftJson();

		services.AddMemoryCache()
		        .AddSupabaseClient(Configuration)
		        .AddWechatApi(Configuration.GetSection("Wechat"))
		        .AddWechatMessageHandler<WechatUserMessageStore>(typeof(Program).Assembly)
		        .AddHostedService<ChatbotBackgroundService>()
		        .AddRecurringJobService();

		services.AddSwaggerGen(gen =>
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

		var corsOrigins = Configuration.GetSection("CorsOrigins").Get<string[]>();
		services.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder.WithOrigins(corsOrigins);
				builder.AllowAnyHeader();
				builder.AllowAnyMethod();
				//builder.AllowCredentials();
			});
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseSwagger();
			app.UseSwaggerUI(option =>
			{
				option.SwaggerEndpoint("/swagger/v1/swagger.json", "v1.0");
			});
		}

		app.UseMiddleware<ExceptionHandlingMiddleware>();

		app.UseHttpsRedirection();

		app.UseRouting()
		   //.UseCors();
		   .UseCors(config =>
		   {
			   config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
		   });

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}