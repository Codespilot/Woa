using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using IdentityModel;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Woa.Common;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class UserService : IUserService
{
	private readonly SupabaseClient _client;
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;
	private readonly IConfiguration _configuration;
	private readonly ILogger<UserService> _logger;

	public UserService(SupabaseClient client, IMediator mediator, IMapper mapper, IConfiguration configuration, ILoggerFactory logger)
	{
		_client = client;
		_mediator = mediator;
		_mapper = mapper;
		_configuration = configuration;
		_logger = logger.CreateLogger<UserService>();
	}

	public async Task<LoginResponseDto> AuthenticateAsync(string username, string password)
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
			throw new BadRequestException("Token错误");
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
			throw new NotFoundException("无效的Token");
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
			throw new BadRequestException("Id必须大于0");
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
			throw new BadRequestException("用户名不能为空");
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
			throw new BadRequestException("Id必须大于0");
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
			throw new NotFoundException("用户不存在");
		}

		return _mapper.Map<UserProfileDto>(entity);
	}

	public async Task<int> CreateAsync(UserRegisterDto model)
	{
		if (model == null)
		{
			throw new BadRequestException("参数不能为空");
		}

		var command = _mapper.Map<UserCreateCommand>(model);
		var id = await _mediator.Send(command);
		if (id <= 0)
		{
			throw new InternalServerException("创建用户失败");
		}

		return id;
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