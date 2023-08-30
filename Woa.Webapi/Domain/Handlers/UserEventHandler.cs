namespace Woa.Webapi.Domain;

public class UserEventHandler : IEventHandler<UserLoginFaultEvent>,
                                IEventHandler<UserLoginSuccessEvent>
{
	private readonly IRepository<UserEntity, int> _repository;
	private readonly ILogger<UserEventHandler> _logger;

	public UserEventHandler(IRepository<UserEntity, int> repository, ILoggerFactory logger)
	{
		_repository = repository;
		_logger = logger.CreateLogger<UserEventHandler>();
	}

	public async Task Handle(UserLoginFaultEvent notification, CancellationToken cancellationToken)
	{
		try
		{
			var entity = await _repository.GetAsync(notification.UserId, cancellationToken);
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

			await _repository.UpdateAsync(entity, cancellationToken);
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
			var entity = await _repository.GetAsync(notification.UserId, cancellationToken);
			if (entity == null)
			{
				return;
			}

			entity.AccessFailedCount = 0;
			entity.LockoutTime = null;

			await _repository.UpdateAsync(entity, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "更新用户登录信息失败");
		}
	}
}