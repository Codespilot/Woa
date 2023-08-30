﻿using System.Globalization;
using Woa.Sdk.Wechat;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 微信事件消息处理器
/// </summary>
[WechatMessageHandle(WechatMessageType.Event)]
public class WechatEventMessageHandler : IWechatMessageHandler
{
	private const string EVENT_TYPE_SUBSCRIBE = "subscribe";
	private const string EVENT_TYPE_UNSUBSCRIBE = "unsubscribe";
	private const string EVENT_TYPE_SCAN = "scan";
	private const string EVENT_TYPE_LOCATION = "location";
	private const string EVENT_TYPE_CLICK = "click";
	private const string EVENT_TYPE_VIEW = "view";

	private readonly IRepository<WechatFollowerEntity, long> _repository;

	public WechatEventMessageHandler(IRepository<WechatFollowerEntity, long> repository)
	{
		_repository = repository;
	}

	/// <inheritdoc />
	public Task<WechatMessage> HandleAsync(WechatMessage message, CancellationToken cancellationToken = default)
	{
		var eventType = message.GetValue<string>(WechatMessageKey.Event.EventType)?.ToLower(CultureInfo.CurrentCulture);
		var openId = message.GetValue<string>(WechatMessageKey.FromUserName);
		var task = eventType switch
		{
			EVENT_TYPE_SUBSCRIBE => OnUserSubscribedAsync(openId, message.CreateTime),
			EVENT_TYPE_UNSUBSCRIBE => OnUserUnsubscribedAsync(openId, message.CreateTime),
			_ => Task.FromResult(WechatMessage.Empty)
		};
		return task;
	}

	/// <summary>
	/// 用户关注公众号
	/// </summary>
	/// <param name="openId"></param>
	/// <param name="operateTime"></param>
	/// <returns></returns>
	private async Task<WechatMessage> OnUserSubscribedAsync(string openId, DateTime operateTime)
	{
		var follower = await _repository.GetAsync(t => t.OpenId == openId);
		follower ??= new WechatFollowerEntity { OpenId = openId, State = 1, SubscribeTime = operateTime };

		if (follower.Id < 1)
		{
			await _repository.InsertAsync(follower);
		}
		else
		{
			await _repository.UpdateAsync(follower);
		}

		return WechatMessage.Text("欢迎关注公众号");
	}

	/// <summary>
	/// 用户取消关注公众号
	/// </summary>
	/// <param name="openId"></param>
	/// <param name="operateTime"></param>
	/// <returns></returns>
	private async Task<WechatMessage> OnUserUnsubscribedAsync(string openId, DateTime operateTime)
	{
		var entity = await _repository.GetAsync(t => t.OpenId == openId);
		if (entity != null)
		{
			entity.State = 1;
			entity.UnfollowTime = operateTime;
		}

		await _repository.UpdateAsync(entity);

		return WechatMessage.Empty;
	}

	/// <summary>
	/// 用户点击自定义菜单
	/// </summary>
	/// <param name="openId"></param>
	/// <param name="menuKey"></param>
	/// <returns></returns>
	public async Task<WechatMessage> OnUserClickedMenuAsync(string openId, string menuKey)
	{
		return await Task.FromResult(WechatMessage.Empty);
	}
}