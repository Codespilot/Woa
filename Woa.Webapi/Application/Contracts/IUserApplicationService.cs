﻿using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public interface IUserApplicationService
{
	Task<LoginResponseDto> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

	Task<LoginResponseDto> AuthenticateAsync(string token, CancellationToken cancellationToken = default);

	Task<UserEntity> GetAsync(int id, CancellationToken cancellationToken = default);

	Task<UserEntity> GetAsync(string username, CancellationToken cancellationToken = default);

	Task<UserProfileDto> GetProfileAsync(int id, CancellationToken cancellationToken = default);

	Task<int> CreateAsync(UserRegisterDto model, CancellationToken cancellationToken = default);
}