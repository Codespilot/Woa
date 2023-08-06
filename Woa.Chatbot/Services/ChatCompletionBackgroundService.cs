using Supabase;
using Supabase.Realtime;
using Supabase.Realtime.Models;
using Woa.Common;

namespace Woa.Chatbot.Services;

public class ChatCompletionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChatCompletionBackgroundService> _logger;

    private RealtimeChannel _channel;

    public ChatCompletionBackgroundService(IServiceProvider provider, IConfiguration configuration, ILoggerFactory logger)
    {
        _provider = provider;
        _configuration = configuration;
        _logger = logger.CreateLogger<ChatCompletionBackgroundService>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var key = _configuration["Supabase:Key"];
            var url = _configuration["Supabase:Url"];
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            };
            var client = new SupabaseClient(url!, key!, options);
            await client.InitializeAsync();
            _channel = client.Realtime.Channel("chatbot");
            var broadcast = _channel.Register<ChatbotBroadcast>();
            broadcast.AddBroadcastEventHandler((_, _) =>
            {
                var payload = broadcast.Current();

                _logger.LogInformation("收到广播: {Payload}", payload);

                if (string.IsNullOrWhiteSpace(payload?.MessageContent))
                {
                    return;
                }

                InvokeChatCompletion(payload.OpenId, payload.MessageId, payload.MessageContent);
            });
            await _channel.Subscribe();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "初始化失败");
        }
    }

    private async void InvokeChatCompletion(string openId, string messageId, string text)
    {
        var content = await Task.Run(async () =>
        {
            try
            {
                var name = _configuration["Bot:Name"];

                var service = _provider.GetRequiredService<IChatCompletionService>(name);

                if (service == null)
                {
                    throw new NullReferenceException("");
                }

                return await service.CreateCompletionAsync(text);
            }
            catch (OperationCanceledException exception)
            {
                _logger.LogError(exception, "请求超时");
                return "无法处理您的请求";
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "请求失败");
                return "发生错误了";
            }
        });

        await _channel.Send(Constants.ChannelEventName.Broadcast, null, new ChatbotBroadcast { MessageContent = content, MessageId = messageId, OpenId = openId });
    }
}

public class ChatbotBroadcast : BaseBroadcast
{
    public string OpenId { get; set; }

    public string MessageId { get; set; }

    public string MessageContent { get; set; }

    public override string ToString()
    {
        return $"{OpenId} ({MessageId}){MessageContent}";
    }
}