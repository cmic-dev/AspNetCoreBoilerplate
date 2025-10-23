using AspNetCoreBoilerplate.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AspNetCoreBoilerplate.Shared.RateLimiter;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreBoilerplate.Modules.Auth.Application.Services;
using AspNetCoreBoilerplate.Shared;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Shared.Authorization;

namespace AspNetCoreBoilerplate.Modules.Auth.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class AuthController : AppControllerBase
{
    private readonly AuthService _authService;
    private readonly IUserContext _userContext;

    public AuthController(AuthService authService, IUserContext userContext)
    {
        _authService = authService;
        _userContext = userContext;
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimiterPolicies.Auth)]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken ctn)
    {
        return Ok(await _authService.LoginAsync(request, ctn));
    }

    [HttpPost("refresh-token")]
    [EnableRateLimiting(RateLimiterPolicies.Auth)]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken ctn)
    {
        return Ok(await _authService.RefreshTokenAsync(request, ctn));
    }

    [HttpPost("logout")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult> Logout(LogoutRequestDto request, CancellationToken ctn)
    {
        await _authService.LogoutAsync(request, ctn);
        return Ok();
    }

    [HttpPost("logout-all-devices")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult> LogoutAllDevices(CancellationToken ctn)
    {
        await _authService.LogoutAllDevicesAsync(_userContext.UserId!.Value, ctn);
        return Ok();
    }

    [HttpPost("change-password")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken ctn)
    {
        await _authService.ChangePasswordAsync(_userContext.UserId!.Value, request, ctn);
        return Ok();
    }

    [HttpPost("revoke-token")]
    [EnableRateLimiting(RateLimiterPolicies.Auth)]
    public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenRequestDto request, CancellationToken ctn)
    {
        await _authService.RevokeTokenAsync(request, ctn);
        return Ok();
    }

    [HttpGet("active-sessions")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<IEnumerable<ActiveSessionDto>>> GetActiveSessions(CancellationToken ctn)
    {
        return Ok(await _authService.GetActiveSessionsAsync(_userContext.UserId!.Value, ctn));
    }

    [HttpGet("login-histories")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<IEnumerable<LoginHistoryDto>>> GetLoginHistories([FromQuery] GetLoginHistoriesRequestDto request, CancellationToken ctn)
    {
        return Ok(await _authService.GetLoginHistoriesAsync(request, ctn));
    }
}
