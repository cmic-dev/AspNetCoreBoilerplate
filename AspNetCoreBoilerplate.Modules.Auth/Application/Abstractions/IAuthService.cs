using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using System;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ctn = default);
    Task LogoutAsync(LogoutRequestDto request, CancellationToken ctn = default);
    Task LogoutAllDevicesAsync(Guid userId, CancellationToken ctn = default);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken ctn = default);
    Task<IEnumerable<LoginHistoryDto>> GetLoginHistoriesAsync(GetLoginHistoriesRequestDto request, CancellationToken ctn = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ctn = default);
    Task RevokeTokenAsync(RevokeTokenRequestDto request, CancellationToken ctn = default);
    Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync(Guid userId, CancellationToken ctn = default);
}
