using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Postgrest;
using Woa.Common;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class UserService : IUserService
{
	private readonly SupabaseClient _client;
	private readonly IMediator _mediator;
	private readonly IConfiguration _configuration;
	private readonly ILogger<UserService> _logger;

	public UserService(SupabaseClient client, IMediator mediator, IConfiguration configuration, ILoggerFactory logger)
	{
		_client = client;
		_mediator = mediator;
		_configuration = configuration;
		_logger = logger.CreateLogger<UserService>();
	}

	public async Task<LoginResponseDto> AuthenticateAsync(string username, string password)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			throw new ArgumentNullException(nameof(username));
		}

		if (string.IsNullOrWhiteSpace(password))
		{
			throw new ArgumentNullException(nameof(password));
		}

		username = username.Trim().ToLower(CultureInfo.CurrentCulture);

		var entity = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Where(t => t.Username == username)
			                                .Single()
		                         );
		if (entity == null || entity.IsDeleted)
		{
			throw new RowNotInTableException("用户名或密码错误");
		}

		if (entity.LockoutTime > DateTime.UtcNow)
		{
			throw new UnauthorizedAccessException("账号已锁定");
		}

		var hash = Cryptography.DES.Encrypt(password, Encoding.UTF8.GetBytes(entity.PasswordSalt));
		if (!string.Equals(entity.PasswordHash, hash))
		{
			await _mediator.Publish(new UserLoginFaultEvent(entity.Id));
			throw new InvalidOperationException("用户名或密码错误");
		}

		var refreshToken = await GenerateRefreshTokenAsync(entity.Id, entity.Username);

		await _mediator.Publish(new UserLoginSuccessEvent(entity.Id));

		var (token, expiresAt) = GenerateAccessToken(entity.Id, entity.Username);

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

	public async Task<LoginResponseDto> AuthenticateAsync(string token)
	{
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new ArgumentNullException(nameof(token), "Token错误");
		}

		token = token.Trim().ToLower(CultureInfo.CurrentCulture);

		var entity = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<RefreshTokenEntity>()
			                                .Where(t => t.Token == token)
			                                .Single()
		                         );

		if (entity == null || !entity.IsValid || entity.ExpiredAt < DateTime.UtcNow)
		{
			throw new RowNotInTableException("无效的Token");
		}

		var refreshToken = await GenerateRefreshTokenAsync(entity.Id, entity.Username);

		var (accessToken, expiresAt) = GenerateAccessToken(entity.UserId, entity.Username);

		var response = new LoginResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken,
			TokenType = "Bearer",
			ExpiresAt = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
			Id = entity.Id,
			Username = entity.Username,
		};

		await _mediator.Publish(new RefreshTokenUsedEvent(token));
		
		return response;
	}

	public async Task<UserEntity> GetAsync(int id)
	{
		if (id <= 0)
		{
			throw new ArgumentNullException(nameof(id), "Id必须大于0");
		}

		var entity = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Where(t => t.Id == id)
			                                .Single()
		                         );
		if (entity == null)
		{
			throw new RowNotInTableException();
		}

		return entity;
	}

	public async Task<UserEntity> GetAsync(string username)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			throw new ArgumentNullException(nameof(username), "用户名不能为空");
		}

		username = username.Trim().ToLower(CultureInfo.CurrentCulture);

		var entity = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Where(t => t.Username == username)
			                                .Single()
		                         );
		if (entity == null)
		{
			throw new RowNotInTableException();
		}

		return entity;
	}

	public async Task<UserProfileDto> GetProfileAsync(int id)
	{
		if (id <= 0)
		{
			throw new ArgumentNullException(nameof(id), "Id必须大于0");
		}

		var entity = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Where(t => t.Id == id)
			                                .Single()
		                         );

		if (entity == null || entity.IsDeleted)
		{
			throw new RowNotInTableException();
		}

		var dto = new UserProfileDto
		{
			Id = entity.Id,
			Username = entity.Username,
			Email = entity.Email,
			Phone = entity.Phone,
			Avatar = entity.Avatar
		};

		return dto;
	}

	public async Task<UserEntity> CreateAsync(UserRegisterDto model)
	{
		if (model == null)
		{
			throw new ArgumentNullException(nameof(model));
		}

		if (string.IsNullOrWhiteSpace(model.Username))
		{
			throw new ArgumentNullException(nameof(model.Username));
		}
		else
		{
			model.Username = model.Username.Trim().ToLower(CultureInfo.CurrentCulture);
			var exists = await CheckExistsAsync(t => t.Username == model.Username);
			if (exists)
			{
				throw new DuplicateNameException("用户名已存在");
			}
		}

		if (string.IsNullOrWhiteSpace(model.Password))
		{
			throw new ArgumentNullException(nameof(model.Password));
		}

		if (!string.IsNullOrWhiteSpace(model.Email))
		{
			model.Email = model.Email.Trim().ToLower(CultureInfo.CurrentCulture);
			var exists = await CheckExistsAsync(t => t.Email == model.Email);
			if (exists)
			{
				throw new DuplicateNameException("邮箱已被注册");
			}
		}

		if (!string.IsNullOrWhiteSpace(model.Phone))
		{
			model.Phone = model.Phone.Trim().ToLower(CultureInfo.CurrentCulture);
			var exists = await CheckExistsAsync(t => t.Phone == model.Phone);
			if (exists)
			{
				throw new DuplicateNameException("手机号已被注册");
			}
		}

		var salt = RandomUtility.CreateUniqueId();
		var hash = Cryptography.DES.Encrypt(model.Password, Encoding.UTF8.GetBytes(salt));

		var entity = new UserEntity
		{
			Username = model.Username.Trim().ToLower(CultureInfo.CurrentCulture),
			PasswordSalt = salt,
			PasswordHash = hash,
			LockoutTime = DateTime.UtcNow,
			IsDeleted = false,
		};

		var result = await Policy.Handle<Exception>()
		                         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                         .ExecuteAsync(() =>
			                         _client.From<UserEntity>()
			                                .Insert(entity)
		                         );

		return result.Model;
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

	private Tuple<string, DateTime> GenerateAccessToken(int userId, string userName)
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

		var token = tokenHandler.CreateToken(tokenDescriptor);
		var tokenString = tokenHandler.WriteToken(token);
		return Tuple.Create(tokenString, expiresAt);
	}

	private async Task<string> GenerateRefreshTokenAsync(int userId, string userName)
	{
		var token = RandomUtility.CreateUniqueId();
		var expiresAt = DateTime.UtcNow.AddDays(30);
		var entity = new RefreshTokenEntity
		{
			UserId = userId,
			Username = userName,
			Token = token,
			ExpiredAt = expiresAt, 
			IsValid = true
		};

		await Policy.Handle<Exception>()
		            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		            .ExecuteAsync(() =>
			            _client.From<RefreshTokenEntity>()
			                   .Insert(entity)
		            );

		return token;
	}

	private void OnRetry(Exception exception, TimeSpan timeSpan, int retryCount, object context)
	{
		_logger.LogError(exception, "第{RetryCount}次重试，等待{TimeSpan}后重试", retryCount, timeSpan);
	}
}