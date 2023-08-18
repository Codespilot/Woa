using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using Woa.Common;
using Woa.Sdk.Claude;
using Woa.Sdk.OpenAi;
using Woa.Sdk.Wechat;

namespace Woa.Sdk;

internal static class ServiceCollectionExtensions
{
	internal static IServiceCollection AddOpenAiApi(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<OpenAiOptions>(configuration);

		services.AddRefitClient<IOpenAiApi>()
		        .ConfigureHttpClient((provider, client) =>
		        {
			        var options = provider.GetRequiredService<IOptions<OpenAiOptions>>()?.Value;
			        if (options == null)
			        {
				        throw new InvalidOperationException($"{nameof(OpenAiOptions)} is not configured");
			        }

			        client.BaseAddress = new Uri(options.Host);
			        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Token);
			        client.DefaultRequestHeaders.Add("openai-organization", options.Organization);
		        });

		return services;
	}

	internal static IServiceCollection AddClaudeApi(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<ClaudeOptions>(configuration);

		services.AddRefitClient<IClaudeApi>()
		        .ConfigureHttpClient((provider, client) =>
		        {
			        var options = provider.GetRequiredService<IOptions<ClaudeOptions>>()?.Value;
			        if (options == null)
			        {
				        throw new InvalidOperationException($"{nameof(ClaudeOptions)} is not configured");
			        }

			        client.BaseAddress = new Uri(options.Host);
			        client.DefaultRequestHeaders.Add("anthropic-version", options.Version);
			        client.DefaultRequestHeaders.Add("x-api-key", options.Key);
		        });
		return services;
	}

	internal static IServiceCollection AddWechatApi(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<WechatOptions>(configuration);

		var settings = new RefitSettings { Buffered = true, ContentSerializer = new NewtonsoftJsonContentSerializer() };
		services.AddRefitClient<IWechatApi>(settings)
		        .ConfigureHttpClient((provider, client) =>
		        {
			        var options = provider.GetRequiredService<IOptions<WechatOptions>>()?.Value;
			        if (options == null)
			        {
				        throw new InvalidOperationException($"{nameof(WechatOptions)} is not configured");
			        }

			        client.BaseAddress = new Uri("https://api.weixin.qq.com");
		        });

		return services;
	}

	internal static IServiceCollection AddWechatMessageHandler<TStore>(this IServiceCollection services, Func<IEnumerable<Type>> handlerTypesFactory)
		where TStore : class, IWechatUserMessageStore
	{
		var handlerTypes = handlerTypesFactory();
		return services.AddWechatMessageHandler<TStore>(handlerTypes);
	}

	internal static IServiceCollection AddWechatMessageHandler<TStore>(this IServiceCollection services, IEnumerable<Type> handlerTypes)
		where TStore : class, IWechatUserMessageStore
	{
		var types = GetWechatMessageHandlers(handlerTypes);

		foreach (var (_, type) in types)
		{
			services.AddScoped(type);
		}

		services.AddTransient<NamedService<IWechatMessageHandler>>(provider =>
		{
			return name =>
			{
				if (types.TryGetValue(name, out var type))
				{
					return provider.GetRequiredService(type) as IWechatMessageHandler;
				}

				return default;
			};
		});
		services.AddScoped<IWechatUserMessageStore, TStore>();
		return services;
	}

	internal static IServiceCollection AddWechatMessageHandler<TStore>(this IServiceCollection services, Assembly assembly)
		where TStore : class, IWechatUserMessageStore
	{
		var handlerTypes = assembly.GetTypes().Where(type => !type.IsAbstract && typeof(IWechatMessageHandler).IsAssignableFrom(type));
		return services.AddWechatMessageHandler<TStore>(handlerTypes);
	}

	private static Dictionary<string, Type> GetWechatMessageHandlers(IEnumerable<Type> handlerTypes)
	{
		var handlers = new Dictionary<string, Type>();
		foreach (var type in handlerTypes)
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