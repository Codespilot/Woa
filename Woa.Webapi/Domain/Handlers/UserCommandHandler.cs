using System.Linq.Expressions;
using System.Text;
using Woa.Common;

namespace Woa.Webapi.Domain;

public class UserCommandHandler : ICommandHandler<UserCreateCommand, int>
{
	private readonly IRepository<UserEntity, int> _repository;

	public UserCommandHandler(IRepository<UserEntity, int> repository)
	{
		_repository = repository;
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

		var result = await _repository.InsertAsync(entity, cancellationToken);
		return result.Id;
	}

	private async Task<bool> CheckExistsAsync(Expression<Func<UserEntity, bool>> predicate)
	{
		return await _repository.ExistsAsync(predicate);
	}
}