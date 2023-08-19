using System.Linq.Expressions;
using System.Text;
using Polly;
using Postgrest;
using Woa.Common;

namespace Woa.Webapi.Domain;

public class UserCommandHandler : ICommandHandler<UserCreateCommand, int>
{
	private readonly SupabaseClient _client;
	private readonly ILogger<UserCommandHandler> _logger;

	public UserCommandHandler(SupabaseClient client, ILoggerFactory logger)
	{
		_client = client;
		_logger = logger.CreateLogger<UserCommandHandler>();
	}

	public async Task<int> Handle(UserCreateCommand request, CancellationToken cancellationToken)
	{
		{
			var exists = await CheckExistsAsync(t => t.Username == request.Username);
			if (exists)
			{
				throw new BadRequestException("用户名已存在");
			}
		}

		if (!string.IsNullOrWhiteSpace(request.Email))
		{
			var exists = await CheckExistsAsync(t => t.Email == request.Email);
			if (exists)
			{
				throw new BadRequestException("邮箱已被注册");
			}
		}

		if (!string.IsNullOrWhiteSpace(request.Phone))
		{
			var exists = await CheckExistsAsync(t => t.Phone == request.Phone);
			if (exists)
			{
				throw new BadRequestException("手机号已被注册");
			}
		}

		var salt = RandomUtility.CreateUniqueId();
		var hash = Cryptography.DES.Encrypt(request.Password, Encoding.UTF8.GetBytes(salt));

		var entity = new UserEntity
		{
			Username = request.Username,
			PasswordSalt = salt,
			PasswordHash = hash,
			LockoutTime = DateTime.UtcNow,
			IsDeleted = false,
		};

		var result = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Insert(entity, cancellationToken: cancellationToken)
		                         );
		return result?.Model?.Id ?? 0;
	}

	private async Task<bool> CheckExistsAsync(Expression<Func<UserEntity, bool>> predicate)
	{
		var result = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Where(predicate)
			                                .Count(Constants.CountType.Exact)
		                         );
		return result > 0;
	}

	private void OnRetry(Exception exception, TimeSpan timeSpan, int retryCount, object context)
	{
		_logger.LogError(exception, "第{RetryCount}次重试，等待{TimeSpan}后重试", retryCount, timeSpan);
	}
}