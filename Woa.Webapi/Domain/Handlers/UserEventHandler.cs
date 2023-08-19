namespace Woa.Webapi.Domain;

public class UserEventHandler : IEventHandler<UserLoginFaultEvent>,
                                IEventHandler<UserLoginSuccessEvent>,
                                IEventHandler<RefreshTokenUsedEvent>
{
	private readonly SupabaseClient _client;
	private readonly ILogger<UserEventHandler> _logger;

	public UserEventHandler(SupabaseClient client, ILoggerFactory logger)
	{
		_client = client;
		_logger = logger.CreateLogger<UserEventHandler>();
	}

	public async Task Handle(UserLoginFaultEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var entity = await _client.From<UserEntity>()
			                          .Where(t => t.Id == notification.UserId)
			                          .Single(cancellationToken);
			if (entity == null)
			{
				return;
			}

			entity.AccessFailedCount++;
			if (entity.AccessFailedCount > 10)
			{
				var seed = entity.AccessFailedCount - 10;
				entity.LockoutTime = DateTime.UtcNow.AddMinutes(5 * Math.Pow(2, seed));
			}

			await _client.From<UserEntity>().Update(entity, cancellationToken: cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "更新用户登录信息失败");
		}
	}

	public async Task Handle(UserLoginSuccessEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			await _client.From<UserEntity>()
			             .Where(t => t.Id == notification.UserId)
			             .Set(t => t.AccessFailedCount, 0)
			             .Set(t => t.LockoutTime, null)
			             .Update(cancellationToken: cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "更新用户登录信息失败");
		}
	}

	public async Task Handle(RefreshTokenUsedEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			// 删除RefreshToken
			await _client.From<RefreshTokenEntity>()
			             .Where(t => t.Token == notification.Token)
			             .Delete(cancellationToken: cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "更新用户登录信息失败");
		}
	}
}