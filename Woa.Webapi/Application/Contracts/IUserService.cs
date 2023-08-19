using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public interface IUserService
{
	Task<LoginResponseDto> AuthenticateAsync(string username, string password);

	Task<LoginResponseDto> AuthenticateAsync(string token);

	Task<UserEntity> GetAsync(int id);

	Task<UserEntity> GetAsync(string username);

	Task<UserProfileDto> GetProfileAsync(int id);

	Task<int> CreateAsync(UserRegisterDto model);
}