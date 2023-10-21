﻿using Woa.Transit;
using Woa.Webapi.Domain;

namespace Woa.Webapi.Application;

public interface IUserApplicationService : IApplicationService
{
	Task<LoginResponseDto> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

	Task<LoginResponseDto> AuthenticateAsync(string token, CancellationToken cancellationToken = default);

	Task<UserDetailDto> GetAsync(int id, CancellationToken cancellationToken = default);

	Task<UserDetailDto> GetAsync(string username, CancellationToken cancellationToken = default);

	Task<UserProfileDto> GetProfileAsync(int id, CancellationToken cancellationToken = default);

	Task<int> CreateAsync(UserRegisterDto model, CancellationToken cancellationToken = default);

	Task<List<UserItemDto>> SearchAsync(UserQueryDto condition, int page, int size, CancellationToken cancellationToken = default);
}