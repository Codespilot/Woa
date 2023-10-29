namespace Woa.Webapi.Domain;

public class UserEventHandler : IEventHandler<UserLoginFaultEvent>,
								IEventHandler<UserLoginSuccessEvent>,
								IEventHandler<RoleDeletedEvent>
{
	private readonly IServiceProvider _provider;
	private readonly ILogger<UserEventHandler> _logger;

	public UserEventHandler(IServiceProvider provider, ILoggerFactory logger)
	{
		_provider = provider;
		_logger = logger.CreateLogger<UserEventHandler>();
	}

	public async Task Handle(UserLoginFaultEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var repository = _provider.GetService<UserRepository>();

			var entity = await repository.GetAsync(notification.UserId, cancellationToken);
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

			await repository.UpdateAsync(entity, cancellationToken);
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
			var repository = _provider.GetService<UserRepository>();

			var entity = await repository.GetAsync(notification.UserId, cancellationToken);
			if (entity == null)
			{
				return;
			}

			entity.AccessFailedCount = 0;
			entity.LockoutTime = null;

			await repository.UpdateAsync(entity, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "更新用户登录信息失败");
		}
	}

	public async Task Handle(RoleDeletedEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var repository = _provider.GetService<UserRoleRepository>();
			await repository.DeleteByRoleIdAsync(notification.Id, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "更新用户角色信息失败");
		}
	}
}