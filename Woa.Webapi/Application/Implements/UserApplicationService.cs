using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AutoMapper;
using IdentityModel;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Woa.Common;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class UserApplicationService : IUserApplicationService
{
	private readonly IRepository<UserEntity, int> _repository;
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;
	private readonly IConfiguration _configuration;

	public UserApplicationService(IRepository<UserEntity, int> repository, IMediator mediator, IMapper mapper, IConfiguration configuration)
	{
		_repository = repository;
		_mediator = mediator;
		_mapper = mapper;
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
			await _mediator.Publish(new UserLoginFaultEvent(entity.Id), cancellationToken);
			throw new InvalidOperationException("用户名或密码错误");
		}

		var refreshToken = GenerateRefreshToken(entity.Id);

		await _mediator.Publish(new UserLoginSuccessEvent(entity.Id), cancellationToken);

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

	public async Task<LoginResponseDto> AuthenticateAsync(string token, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(token))
		{
			throw new BadRequestException("Token错误");
		}

		token = token.Trim().ToLower(CultureInfo.CurrentCulture);

		var json = Cryptography.AES.Decrypt(token);

		var model = JsonSerializer.Deserialize<RefreshTokenModel>(json);

		var entity = await _repository.GetAsync(model.Id, cancellationToken);

		if (entity == null || entity.IsDeleted)
		{
			throw new NotFoundException("用户名或密码错误");
		}

		var refreshToken = GenerateRefreshToken(entity.Id);

		var (accessToken, expiresAt) = GenerateAccessToken(entity.Id, entity.Username);

		var response = new LoginResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken,
			TokenType = "Bearer",
			ExpiresAt = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
			Id = entity.Id,
			Username = entity.Username,
		};

		await _mediator.Publish(new RefreshTokenUsedEvent(token), cancellationToken);

		return response;
	}

	public async Task<UserEntity> GetAsync(int id, CancellationToken cancellationToken = default)
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

		return entity;
	}

	public async Task<UserEntity> GetAsync(string username, CancellationToken cancellationToken = default)
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

		return entity;
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

		return _mapper.Map<UserProfileDto>(entity);
	}

	public async Task<int> CreateAsync(UserRegisterDto model, CancellationToken cancellationToken = default)
	{
		if (model == null)
		{
			throw new BadRequestException("参数不能为空");
		}

		var command = _mapper.Map<UserCreateCommand>(model);
		var id = await _mediator.Send(command, cancellationToken);
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

	private static string GenerateRefreshToken(int userId)
	{
		var expiresAt = DateTime.UtcNow.AddDays(30);
		var obj = new RefreshTokenModel
		{
			Id = userId,
			Expiry = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
			Rdm = Guid.NewGuid().ToString()
		};
		var json = JsonSerializer.Serialize(obj);
		return Cryptography.AES.Encrypt(json);
	}

	public class RefreshTokenModel
	{
		public int Id { get; set; }

		public long Expiry { get; set; }

		public string Rdm { get; set; }
	}
}