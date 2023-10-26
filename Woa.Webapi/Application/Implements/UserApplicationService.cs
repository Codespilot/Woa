using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Woa.Common;
using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public class UserApplicationService : BaseApplicationService, IUserApplicationService
{
	private readonly IRepository<UserEntity, int> _repository;
	private readonly IConfiguration _configuration;

	public UserApplicationService(IRepository<UserEntity, int> repository, IConfiguration configuration)
	{
		_repository = repository;
		_configuration = configuration;
	}

	public async Task<LoginResponseDto> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			throw new BadRequestException("用户名不能为空");
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			throw new BadRequestException("密码不能为空");
		}

		username = username.Trim().ToLower(CultureInfo.CurrentCulture);

		var entity = await _repository.GetAsync(t => t.Username == username, cancellationToken);
		if (entity == null || entity.IsDeleted)
		{
			throw new NotFoundException("用户名或密码错误");
		}

		if (entity.LockoutTime > DateTime.UtcNow)
		{
			throw new UnauthorizedAccessException("账号已锁定");
		}

		var hash = Cryptography.DES.Encrypt(password, Encoding.UTF8.GetBytes(entity.PasswordSalt));
		if (!string.Equals(entity.PasswordHash, hash))
		{
			await Mediator.Publish(new UserLoginFaultEvent(entity.Id), cancellationToken);
			throw new InvalidOperationException("用户名或密码错误");
		}

		var roles = await GetRolesAsync(entity.Id, t => t.Code, cancellationToken);

		var refreshToken = GenerateRefreshToken(entity.Id);

		await Mediator.Publish(new UserLoginSuccessEvent(entity.Id), cancellationToken);

		var (token, expiresAt) = GenerateAccessToken(entity.Id, entity.Username, roles);

		var response = new LoginResponseDto
		{
			AccessToken = token,
			RefreshToken = refreshToken,
			TokenType = "Bearer",
			ExpiresAt = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
			Id = entity.Id,
			Username = entity.Username,
		};

		return response;
	}

	public async Task<LoginResponseDto> AuthenticateAsync(string token, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new BadRequestException("Token错误");
		}

		token = token.Trim().ToLower(CultureInfo.CurrentCulture);

		var json = Cryptography.AES.Decrypt(token);

		var document = JsonSerializer.Deserialize<JsonDocument>(json);

		var id = ReadJson<int>(document.RootElement, "Id");
		var expiry = ReadJson<long>(document.RootElement, "Expiry");

		var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

		if (expiry < timestamp)
		{
			throw new BadRequestException("Token已失效");
		}

		var entity = await _repository.GetAsync(id, cancellationToken);

		if (entity == null || entity.IsDeleted)
		{
			throw new NotFoundException("用户名或密码错误");
		}

		var roles = await GetRolesAsync(entity.Id, t => t.Code, cancellationToken);

		var refreshToken = GenerateRefreshToken(entity.Id);

		var (accessToken, expiresAt) = GenerateAccessToken(entity.Id, entity.Username, roles);

		var response = new LoginResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken,
			TokenType = "Bearer",
			ExpiresAt = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
			Id = entity.Id,
			Username = entity.Username,
		};

		return response;

		static TValue ReadJson<TValue>(JsonElement element, string property)
		{
			var value = element.GetProperty(property);
			return JsonSerializer.Deserialize<TValue>(value.GetRawText());
		}
	}

	public async Task<UserDetailDto> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		if (id <= 0)
		{
			throw new BadRequestException("Id必须大于0");
		}

		var entity = await _repository.GetAsync(id, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException();
		}

		var dto = Mapper.Map<UserDetailDto>(entity);
		dto.Roles = await GetRolesAsync(id, t => t.Name, cancellationToken).ContinueWith(task => task.Result.ToList(), cancellationToken);
		return dto;
	}

	public async Task<UserDetailDto> GetAsync(string username, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			throw new BadRequestException("用户名不能为空");
		}

		username = username.Trim().ToLower(CultureInfo.CurrentCulture);

		var entity = await _repository.GetAsync(t => t.Username == username, cancellationToken);
		if (entity == null)
		{
			throw new NotFoundException();
		}

		var dto = Mapper.Map<UserDetailDto>(entity);
		dto.Roles = await GetRolesAsync(entity.Id, t => t.Name, cancellationToken)
			.ContinueWith(task => task.Result.ToList(), cancellationToken);
		return dto;
	}

	public async Task<UserProfileDto> GetProfileAsync(int id, CancellationToken cancellationToken = default)
	{
		if (id <= 0)
		{
			throw new BadRequestException("Id必须大于0");
		}

		var entity = await _repository.GetAsync(id, cancellationToken);

		if (entity == null || entity.IsDeleted)
		{
			throw new NotFoundException("用户不存在");
		}

		return Mapper.Map<UserProfileDto>(entity);
	}

	public async Task<int> CreateAsync(UserCreateDto model, CancellationToken cancellationToken = default)
	{
		if (model == null)
		{
			throw new BadRequestException("参数不能为空");
		}

		var command = Mapper.Map<UserCreateCommand>(model);
		var id = await Mediator.Send(command, cancellationToken);
		if (id <= 0)
		{
			throw new InternalServerException("创建用户失败");
		}

		return id;
	}

	public async Task<List<UserItemDto>> SearchAsync(UserQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
	{
		var expressions = new List<Expression<Func<UserEntity, bool>>>();

		if (!string.IsNullOrWhiteSpace(condition.Keywords))
		{
			expressions.Add(t => t.Username.Contains(condition.Keywords) || t.Email.Contains(condition.Keywords) || t.Phone.Contains(condition.Keywords));
		}

		switch (condition.Locked)
		{
			case true:
				expressions.Add(t => t.LockoutTime > DateTime.UtcNow);
				break;
			case false:
				expressions.Add(t => t.LockoutTime == null || t.LockoutTime < DateTime.UtcNow);
				break;
		}

		var predicate = expressions.Aggregate(t => t.Id > 0);

		var entities = await _repository.FindAsync(predicate, page, size, cancellationToken);
		return Mapper.Map<List<UserItemDto>>(entities);
	}

	public Task<int> CountAsync(UserQueryDto condition, CancellationToken cancellationToken = default)
	{
		var expressions = new List<Expression<Func<UserEntity, bool>>>();

		if (!string.IsNullOrWhiteSpace(condition.Keywords))
		{
			expressions.Add(t => t.Username.Contains(condition.Keywords) || t.Email.Contains(condition.Keywords) || t.Phone.Contains(condition.Keywords));
		}

		switch (condition.Locked)
		{
			case true:
				expressions.Add(t => t.LockoutTime > DateTime.UtcNow);
				break;
			case false:
				expressions.Add(t => t.LockoutTime == null || t.LockoutTime < DateTime.UtcNow);
				break;
		}

		var predicate = expressions.Aggregate(t => t.Id > 0);

		return _repository.CountAsync(predicate, cancellationToken);
	}

	/// <summary>
	/// 生成Access Token
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="userName"></param>
	/// <param name="roles"></param>
	/// <returns></returns>
	private Tuple<string, DateTime> GenerateAccessToken(int userId, string userName, IEnumerable<string> roles = null)
	{
		var authTime = DateTime.UtcNow;
		var expiresAt = authTime.AddDays(1);
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenKey = _configuration.GetValue<string>("JwtBearerOptions:TokenKey");
		var key = Encoding.UTF8.GetBytes(tokenKey.ToSha256());
		var issuer = _configuration.GetValue<string>("JwtBearerOptions:TokenIssuer");
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(JwtClaimTypes.Issuer, issuer),
				new Claim(JwtClaimTypes.Subject, userId.ToString()),
				new Claim(JwtClaimTypes.Name, userName)
			}),
			Expires = expiresAt,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};
		if (roles?.Any() == true)
		{
			foreach (var role in roles)
			{
				tokenDescriptor.Subject.AddClaim(new Claim(JwtClaimTypes.Role, role));
			}
		}

		var token = tokenHandler.CreateToken(tokenDescriptor);
		var tokenString = tokenHandler.WriteToken(token);
		return Tuple.Create(tokenString, expiresAt);
	}

	/// <summary>
	/// 生成Refresh Token
	/// </summary>
	/// <param name="userId">用户Id</param>
	/// <returns></returns>
	private static string GenerateRefreshToken(int userId)
	{
		var expiresAt = DateTime.UtcNow.AddDays(30);
		var model = new
		{
			Id = userId,
			Expiry = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
			Rdm = Guid.NewGuid().ToString()
		};
		var json = JsonSerializer.Serialize(model);
		return Cryptography.AES.Encrypt(json);
	}

	private async Task<IEnumerable<TOutput>> GetRolesAsync<TOutput>(int userId, Func<RoleEntity, TOutput> selector, CancellationToken cancellationToken = default)
	{
		var relationRepository = ServiceProvider.GetRequiredService<IRepository<UserRoleEntity, int>>();
		var relations = await relationRepository.FindAsync(t => t.UserId == userId, cancellationToken);
		if (relations.Count == 0)
		{
			return Array.Empty<TOutput>();
		}

		var roleIds = relations.Select(t => t.RoleId as object).ToList();

		var roleRepository = ServiceProvider.GetRequiredService<IRepository<RoleEntity, int>>();

		// Supabase client不支持 xxx.Contains(t.Id)表达式，所以这里改成了下面的写法
		var roles = await roleRepository.FindAsync(t => t.Id, QueryOperator.In, roleIds, cancellationToken);

		return roles.Select(selector);
	}
}