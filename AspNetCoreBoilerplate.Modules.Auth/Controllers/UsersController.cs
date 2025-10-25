using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Authorization;
using AspNetCoreBoilerplate.Shared.RateLimiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreBoilerplate.Modules.Auth.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userProfileService;
    private readonly IUserContext _userContext;
    private readonly ILogger<ProfileController> _logger;

    public UsersController(IUserService userProfileService, IUserContext userContext, ILogger<ProfileController> logger)
    {
        _userProfileService = userProfileService;
        _userContext = userContext;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<Guid>> CreateUser(CreateUserRequestDto dto, CancellationToken ctn = default)
    {
        return Ok(await _userProfileService.CreateUserAsync(dto, ctn));
    }

    [HttpDelete("{userId:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var deactivated = await _userProfileService.DeactivateUserAsync(userId, ctn);

        if (!deactivated)
            return NotFound();

        _logger.LogInformation("Admin user {AdminId} deactivated user {UserId} from {IpAddress}",
            _userContext.UserId, userId, _userContext.IpAddress);

        return Ok();
    }

    [HttpPost("{userId:guid}/activate")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<IActionResult> ActivateUser(Guid userId, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var activated = await _userProfileService.ActivateUserAsync(userId, ctn);

        if (!activated)
            return NotFound();

        _logger.LogInformation("Admin user {AdminId} activated user {UserId} from {IpAddress}",
            _userContext.UserId, userId, _userContext.IpAddress);

        return Ok();
    }
}
