using Microsoft.Extensions.Caching.Memory;
using Quartz;
using Woa.Sdk.Tencent;

namespace Woa.Webapi.Host;

[RecurringJob("WechatAccessTokenGrantJob", Interval = 30, DelaySeconds = 1)]
public class WechatAccessTokenGrantJob : IJob
{
	private readonly IWechatApi _api;
	private readonly WechatOptions _options;
	private readonly IMemoryCache _cache;
	private readonly ILogger<WechatAccessTokenGrantJob> _logger;
	
	public WechatAccessTokenGrantJob(IWechatApi api, WechatOptions options, IMemoryCache cache, ILoggerFactory logger)
	{
		_api = api;
		_options = options;
		_cache = cache;
		_logger = logger.CreateLogger<WechatAccessTokenGrantJob>();
	}

	public async Task Execute(IJobExecutionContext context)
	{
		try
		{
			foreach (var (id, account) in _options.Accounts)
			{
				if (!account.Enabled)
				{
					continue;
				}

				var response = await _api.GrantTokenAsync("client_credential", account.AppId, account.AppSecret);
				if (!response.IsSuccessStatusCode || response.Content == null)
				{
					return;
				}

				_cache.Set($"{Constants.Cache.WechatAccessToken}:{id}", response.Content.Token, TimeSpan.FromSeconds(response.Content.Expiry - 1800));
			}
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "{Message}", exception.Message);
		}
	}
}