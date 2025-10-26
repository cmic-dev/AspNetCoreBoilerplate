using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;

public interface IProfileService
{
    Task<UserDetailsDto?> GetUserProfileAsync(Guid userId, CancellationToken ctn = default);
    Task<UserDetailsDto?> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto dto, CancellationToken ctn = default);
    Task<UserDetailsDto?> UpdateProfilePictureAsync(Guid userId, string pictureUrl, CancellationToken ctn);
    Task ClearProfilePictureAsync(Guid userId, CancellationToken ctn);
}
