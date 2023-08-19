using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Supabase.Realtime;
using Supabase.Realtime.Models;
using Woa.Sdk.Wechat;
using Woa.Shared;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Host;

public class ChatbotBackgroundService : BackgroundService
{
	private readonly SupabaseClient _client;
	private readonly IMemoryCache _cache;
	private readonly IConfiguration _configuration;
	private readonly IWechatApi _api;

	public ChatbotBackgroundService(SupabaseClient client, IMemoryCache cache, IConfiguration configuration, IWechatApi api)
	{
		_client = client;
		_cache = cache;
		_configuration = configuration;
		_api = api;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _client.InitializeAsync();
		var channel = _client.Realtime.Channel("chatbot");
		var broadcast = channel.Register<ChatbotBroadcast>(false, true);
		broadcast.AddBroadcastEventHandler((_, message) =>
		{
			var payload = (ChatbotBroadcast)message;
			if (string.IsNullOrWhiteSpace(payload?.MessageContent))
			{
				return;
			}

			if (_configuration.GetValue<bool>("Wechat:EnableCustomMessage"))
			{
				SendCustomMessage(payload.OpenId, payload.MessageContent);
			}
			else
			{
				_client.From<WechatMessageEntity>()
				       .Where(t => t.Id == payload.MessageId)
				       .Set(t => t.Reply, payload.MessageContent)
				       .Update(cancellationToken: stoppingToken);
			}
		});
		await channel.Subscribe();

		WeakReferenceMessenger.Default.Register<ChatbotBroadcast>(this, SendBroadcastMessage);

		async void SendBroadcastMessage(object sender, ChatbotBroadcast message)
		{
			await channel.Send(Constants.ChannelEventName.Broadcast, null, message);
		}
	}

	private async void SendCustomMessage(string openId, string content)
	{
		if (!_cache.TryGetValue<string>("Wechat:AccessToken", out var accessToken))
		{
			return;
		}

		var reply = new WechatMessage
		{
			["touser"] = openId,
			["msgtype"] = "text",
			["text"] = new
			{
				content
			}
		};
		var response = await _api.SendCustomMessageAsync(accessToken, reply);
		Debug.WriteLine(response);
	}
}