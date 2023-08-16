using Microsoft.Extensions.Caching.Memory;
using Quartz;
using Woa.Sdk.Wechat;

namespace Woa.Webapi.Jobs;

[RecurringJob("WechatAccessTokenGrantJob", Interval = 30, DelaySeconds = 1)]
public class WechatAccessTokenGrantJob : IJob
{
    private readonly IWechatApi _api;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;

    public WechatAccessTokenGrantJob(IWechatApi api, IConfiguration configuration, IMemoryCache cache)
    {
        _api = api;
        _configuration = configuration;
        _cache = cache;
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
        }
        catch
        {
            // ignored
        }
    }
}