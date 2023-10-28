using System.Globalization;
using Woa.Sdk.Tencent;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Wechat;

/// <summary>
/// 微信事件消息处理器
/// </summary>
[WechatMessageHandle(WechatMessageType.Event)]
internal class WechatEventMessageHandler : WechatUserMessageHandler
{
    private const string EVENT_TYPE_SUBSCRIBE = "subscribe";
    private const string EVENT_TYPE_UNSUBSCRIBE = "unsubscribe";
    private const string EVENT_TYPE_SCAN = "scan";
    private const string EVENT_TYPE_LOCATION = "location";
    private const string EVENT_TYPE_CLICK = "click";
    private const string EVENT_TYPE_VIEW = "view";

    private readonly WechatFollowerRepository _repository;

	public WechatEventMessageHandler(WechatFollowerRepository repository, IWechatUserMessageStore store)
		: base(store)
	{
		_repository = repository;
	}

	protected override Task<WechatMessage> HandleMessageAsync(string openId, string platformId, WechatMessage message, CancellationToken cancellationToken = default)
	{
		var eventType = message.GetValue<string>(WechatMessageKey.Event.EventType)?.ToLower(CultureInfo.CurrentCulture);
		var accountId = message.GetValue<string>(WechatMessageKey.ToUserName);
		var task = eventType switch
		{
			EVENT_TYPE_SUBSCRIBE => OnUserSubscribedAsync(openId, accountId, message.CreateTime),
			EVENT_TYPE_UNSUBSCRIBE => OnUserUnsubscribedAsync(openId, accountId, message.CreateTime),
			_ => Task.FromResult(WechatMessage.Empty)
		};
		return task;
	}

	/// <summary>
	/// 用户关注公众号
	/// </summary>
	/// <param name="openId">用户open id</param>
	/// <param name="platformId">微信公众号Id</param>
	/// <param name="operateTime">操作时间</param>
	/// <returns></returns>
	private async Task<WechatMessage> OnUserSubscribedAsync(string openId, string platformId, DateTime operateTime)
    {
		var follower = await _repository.GetAsync(openId, platformId);
        follower ??= new WechatFollowerEntity
        {
            OpenId = openId,
            PlatformId = platformId,
            State = 1,
			CreateTime = DateTime.UtcNow,
            SubscribeTime = operateTime
        };

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
    /// <param name="openId">用户open id</param>
    /// <param name="platformId">微信公众号Id</param>
    /// <param name="operateTime">操作时间</param>
    /// <returns></returns>
    private async Task<WechatMessage> OnUserUnsubscribedAsync(string openId, string platformId, DateTime operateTime)
    {
		var entity = await _repository.GetAsync(openId, platformId);
        if (entity != null)
        {
            entity.State = 1;
            entity.UnsubscribeTime = operateTime;
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