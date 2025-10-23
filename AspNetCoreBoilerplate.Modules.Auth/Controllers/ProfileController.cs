using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Modules.Auth.Application.Services;
using AspNetCoreBoilerplate.Shared;
using AspNetCoreBoilerplate.Shared.Controllers;
using AspNetCoreBoilerplate.Shared.RateLimiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AspNetCoreBoilerplate.Modules.Auth.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProfileController : AppControllerBase
{
    private readonly IUserContext _userContext;
    private readonly ProfileService _profileService;
    public ProfileController(IUserContext userContext, ProfileService profileService)
    {
        _userContext = userContext;
        _profileService = profileService;
    }

    [HttpGet("me")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    [AllowAnonymous]
    public async Task<ActionResult<UserDetailsDto>> GetCurrentUser(CancellationToken ctn)
    {
        var user = await _profileService.GetUserByIdAsync(_userContext.UserId!.Value, ctn);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("update-profile")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<UserDetailsDto>> UpdateProfile(UpdateProfileRequestDto request, CancellationToken ctn = default)
    {
        if (!_userContext.UserId.HasValue)
            return Unauthorized();

        return Ok(await _profileService.UpdateProfileAsync(_userContext.UserId.Value, request, ctn));
    }

    [HttpPost("update-email")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<UserDetailsDto>> ChangeEmailAddress(UpdateEmailRequestDto request, CancellationToken ctn = default)
    {
        if (!_userContext.UserId.HasValue)
            return Unauthorized();

        return Ok(await _profileService.UpdateEmailAddressAsync(_userContext.UserId.Value, request, ctn));
    }

    [HttpPost("update-picture")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<UserDetailsDto>> UpdatePicture(IFormFile picture)
    {
        return Ok("");
    }
}
