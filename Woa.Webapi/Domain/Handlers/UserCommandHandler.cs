using System.Linq.Expressions;
using System.Text;
using Woa.Common;

namespace Woa.Webapi.Domain;

public class UserCommandHandler : ICommandHandler<UserCreateCommand, int>
{
	private readonly UserRepository _repository;

	public UserCommandHandler(UserRepository repository)
	{
		_repository = repository;
	}

	public async Task<int> Handle(UserCreateCommand request, CancellationToken cancellationToken)
	{
		var expressions = new List<Expression<Func<UserEntity, bool>>>();

		if (!string.IsNullOrWhiteSpace(request.Email))
		{
			expressions.Add(t => t.Email == request.Email);
		}

		if (!string.IsNullOrWhiteSpace(request.Phone))
		{
			expressions.Add(t => t.Phone == request.Phone);
		}

		var predicate = expressions.Aggregate(t => t.Username == request.Username, "Or");

		var exists = await _repository.FindAsync(predicate, cancellationToken);

		if (exists.Any(t => string.Equals(t.Username, request.Username, StringComparison.OrdinalIgnoreCase)))
		{
			throw new BadRequestException("用户名已存在");
		}

		if (exists.Any(t => string.Equals(t.Email, request.Email, StringComparison.OrdinalIgnoreCase)))
		{
			throw new BadRequestException("邮箱已被注册");
		}

		if (exists.Any(t => string.Equals(t.Phone, request.Phone, StringComparison.OrdinalIgnoreCase)))
		{
			throw new BadRequestException("手机号已被注册");
		}

		var salt = RandomUtility.CreateUniqueId();
		var hash = Cryptography.DES.Encrypt(request.Password, Encoding.UTF8.GetBytes(salt));

		var entity = new UserEntity
		{
			Username = request.Username,
			Phone = request.Phone,
			Email = request.Email,
			Fullname = request.Fullname,
			PasswordSalt = salt,
			PasswordHash = hash,
			IsDeleted = false, 
			CreateTime = DateTime.UtcNow
		};

		var result = await _repository.InsertAsync(entity, cancellationToken);
		return result.Id;
	}
}