using Polly.Extensions.Http;
using Polly.Timeout;
using Polly;
using System.Net.Sockets;
using Refit;
using System.Net;
using System.Security.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Http;
using System.Net.Http.Handlers;

namespace Woa.Webapp.Rest;

public static class ServiceCollectionExtensions
{
	private static readonly RefitSettings _refitSettings = new()
	{
		ContentSerializer = new NewtonsoftJsonContentSerializer()
		//ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
		//{
		//	NumberHandling = JsonNumberHandling.AllowReadingFromString,
		//	Converters =
		//	{
		//		new DateTimeConverter(),
		//		new NullableDateTimeConverter(),
		//		new JsonStringEnumConverter()
		//	}
		//})
	};

	public static IServiceCollection AddHttpClientApi(this IServiceCollection services, Action<RestServiceOptions> config)
	{
		services.Configure(config);
		services.AddTransient<LoggingHandler>();
		services.AddTransient<SlowRequestHandler>();
		services.AddTransient<AuthorizationHandler>();

		services.AddTransient(provider => provider.GetRestService<IAccountApi>("woa"))
		        .AddTransient(provider => provider.GetRestService<IUserApi>("woa"))
		        .AddTransient(provider => provider.GetRestService<IWechatMessageApi>("woa"));

		services.AddHttpClient("woa", (provider, client) =>
		        {
			        var options = provider.GetService<IOptions<RestServiceOptions>>().Value;
			        client.BaseAddress = new Uri(options.BaseUrl);
			        client.Timeout = options.Timeout;
		        })
		        // .ConfigurePrimaryHttpMessageHandler(() =>
		        // {

		        // 	var handler = new HttpClientHandler
		        // 	{
		        // 		// SslProtocols = SslProtocols.Tls12,
		        // 		// ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
		        // 		// Proxy = WebRequest.DefaultWebProxy, 
		        // 	};

		        // 	return handler;
		        // })
		        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
		        .AddHttpMessageHandler<AuthorizationHandler>()
		        .AddHttpMessageHandler<LoggingHandler>()
		        .AddHttpMessageHandler<SlowRequestHandler>()
				.AddHttpMessageHandler<ProgressMessageHandler>();

		return services;
	}

	private static HttpClient GetHttpClient(this IServiceProvider provider, string name)
	{
		var factory = provider.GetService<IHttpClientFactory>();
		var client = factory.CreateClient(name);
		return client;
	}

	private static TService GetRestService<TService>(this IServiceProvider provider, string name)
	{
		var client = provider.GetHttpClient(name);
		return RestService.For<TService>(client, _refitSettings);
	}

	private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
	{
		return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(15));
	}

	private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
	{
		return HttpPolicyExtensions.HandleTransientHttpError()
		                           .Or<TimeoutRejectedException>()
		                           .Or<SocketException>()
		                           .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
		                           .OrResult(response => response.StatusCode == HttpStatusCode.ServiceUnavailable)
		                           .WaitAndRetryAsync(5,
			                           retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
	}
}