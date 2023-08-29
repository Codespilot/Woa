using Microsoft.Extensions.Caching.Memory;
using Quartz;
using Woa.Sdk.Wechat;

namespace Woa.Webapi.Host;

[RecurringJob("WechatAccessTokenGrantJob", Interval = 30, DelaySeconds = 1)]
public class WechatAccessTokenGrantJob : IJob
{
	private readonly IWechatApi _api;
	private readonly IConfiguration _configuration;
	private readonly IMemoryCache _cache;
	private readonly ILogger<WechatAccessTokenGrantJob> _logger;

	public WechatAccessTokenGrantJob(IWechatApi api, IConfiguration configuration, IMemoryCache cache, ILoggerFactory logger)
	{
		_api = api;
		_configuration = configuration;
		_cache = cache;
		_logger = logger.CreateLogger<WechatAccessTokenGrantJob>();
	}

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			var response = await _api.GrantTokenAsync("client_credential", _configuration["Wechat:AppId"], _configuration["Wechat:AppSecret"]);
			if (!response.IsSuccessStatusCode || response.Content == null)
			{
				return;
			}

			_cache.Set("Wechat:AccessToken", response.Content.Token, TimeSpan.FromSeconds(response.Content.Expiry - 1800));

			_logger.LogDebug("WechatAccessToken:{Token}", response.Content.Token);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "{Message}", exception.Message);
			// ignored
		}
	}
}