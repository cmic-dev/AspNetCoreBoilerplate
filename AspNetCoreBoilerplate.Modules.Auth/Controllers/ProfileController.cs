using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Authorization;
using AspNetCoreBoilerplate.Shared.Exceptions;
using AspNetCoreBoilerplate.Shared.RateLimiter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreBoilerplate.Modules.Auth.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _userProfileService;
    private readonly IUserContext _userContext;
    private readonly IStorageService _storageService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        IProfileService userProfileService,
        IUserContext userContext,
        ILogger<ProfileController> logger,
        IStorageService storageService)
    {
        _userProfileService = userProfileService;
        _userContext = userContext;
        _storageService = storageService;
        _logger = logger;
    }

    [HttpGet("me")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<UserDetailsDto>> GetCurrentUserProfile(CancellationToken ctn = default)
    {
        if (!_userContext.UserId.HasValue)
            return Unauthorized("User context is invalid");

        var userProfile = await _userProfileService.GetUserProfileAsync(_userContext.UserId.Value, ctn);

        if (userProfile == null)
            return NotFound();

        _logger.LogInformation("User {UserId} retrieved their profile from {IpAddress}",
            _userContext.UserId, _userContext.IpAddress);

        return Ok(userProfile);
    }

    [HttpGet("{userId:guid}")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<UserDetailsDto>> GetUserProfile(Guid userId, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var userProfile = await _userProfileService.GetUserProfileAsync(userId, ctn);

        if (userProfile == null)
            return NotFound();

        _logger.LogInformation("User {RequesterId} viewed profile of user {UserId} from {IpAddress}",
            _userContext.UserId, userId, _userContext.IpAddress);

        return Ok(userProfile);
    }

    [HttpGet]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetAllActiveUsers(CancellationToken ctn = default)
    {
        var users = await _userProfileService.GetAllActiveUsersAsync(ctn);

        _logger.LogInformation("Admin user {UserId} retrieved all active users from {IpAddress}",
            _userContext.UserId, _userContext.IpAddress);

        return Ok(users);
    }

    [HttpPut("me")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateUserProfileDto dto, CancellationToken ctn = default)
    {
        if (!_userContext.UserId.HasValue)
            return Unauthorized();

        var userProfile = await _userProfileService.UpdateUserProfileAsync(_userContext.UserId.Value, dto, ctn);

        if (userProfile is null)
            return NotFound();

        _logger.LogInformation("User {UserId} updated their profile from {IpAddress}",
            _userContext.UserId, _userContext.IpAddress);

        return Ok(userProfile);
    }

    [HttpPut("{userId:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UpdateUserProfileDto dto, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        var userProfile = await _userProfileService.UpdateUserProfileAsync(userId, dto, ctn);

        if (userProfile is null)
            return NotFound();

        _logger.LogInformation("Admin user {AdminId} updated profile of user {UserId} from {IpAddress}",
            _userContext.UserId, userId, _userContext.IpAddress);

        return Ok(userProfile);
    }

    [HttpPut("me/picture")]
    [Consumes("multipart/form-data")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<IActionResult> UpdateCurrentUserProfilePicture(IFormFile picture, CancellationToken ctn = default)
    {
        if (!_userContext.UserId.HasValue)
            return Unauthorized();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (picture == null || picture.Length == 0)
            return BadRequest("No file was uploaded");

        var pictureUrl = await SaveProfilePictureAsync(picture, ctn);
        var userProfile = await _userProfileService.UpdateProfilePictureAsync(_userContext.UserId.Value, pictureUrl, ctn);

        if (userProfile is null)
            return NotFound();

        _logger.LogInformation("User {UserId} updated their profile picture from {IpAddress}",
            _userContext.UserId, _userContext.IpAddress);

        return Ok(userProfile);
    }

    [HttpPut("{userId:guid}/picture")]
    [Authorize(Roles = Roles.SuperAdmin)]
    [Consumes("multipart/form-data")]
    [EnableRateLimiting(RateLimiterPolicies.Authenticated)]
    public async Task<IActionResult> UpdateUserProfilePicture(Guid userId, IFormFile picture, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (picture == null || picture.Length == 0)
            return BadRequest("No file was uploaded");

        var pictureUrl = await SaveProfilePictureAsync(picture, ctn);
        var userProfile = await _userProfileService.UpdateProfilePictureAsync(userId, pictureUrl, ctn);

        if (userProfile is null)
            return NotFound();

        _logger.LogInformation("Admin user {AdminId} updated profile picture of user {UserId} from {IpAddress}",
            _userContext.UserId, userId, _userContext.IpAddress);

        return Ok(userProfile);
    }

    [HttpDelete("me/picture")]
    public async Task<IActionResult> DeleteCurrentUserProfilePicture(CancellationToken ctn = default)
    {
        if (!_userContext.UserId.HasValue)
            return Unauthorized();

        var userProfile = await _userProfileService.GetUserProfileAsync(_userContext.UserId.Value, ctn);

        if (userProfile == null)
            return NotFound();

        await _userProfileService.ClearProfilePictureAsync(_userContext.UserId.Value, ctn);

        _logger.LogInformation("User {UserId} deleted their profile picture from {IpAddress}",
            _userContext.UserId, _userContext.IpAddress);

        return Ok();
    }

    private async Task<string> SaveProfilePictureAsync(IFormFile picture, CancellationToken ctn = default)
    {
        const long maxFileSize = 5 * 1024 * 1024; // 5 MB
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };

        if (picture.Length > maxFileSize)
            throw new DomainException($"File size cannot exceed 5 MB", StatusCodes.Status400BadRequest);

        var fileExtension = Path.GetExtension(picture.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
            throw new DomainException(
                $"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}",
                StatusCodes.Status400BadRequest);

        if (!allowedContentTypes.Contains(picture.ContentType))
            throw new DomainException(
                $"Invalid content type. Allowed types: {string.Join(", ", allowedContentTypes)}",
                StatusCodes.Status400BadRequest);

        using (var memoryStream = new MemoryStream())
        {
            await picture.CopyToAsync(memoryStream, ctn);
            var fileData = memoryStream.ToArray();

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var pictureUrl = await _storageService.UploadFileAsync(fileData, fileName, picture.ContentType, ctn);

            return pictureUrl;
        }
    }
}
