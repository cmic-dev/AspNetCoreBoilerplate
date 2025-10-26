using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Services;

internal class ProfileService : IProfileService
{
    private readonly IAppDbContext _dbContext;
    public ProfileService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDetailsDto?> GetUserProfileAsync(Guid userId, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var user = await _dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => new UserDetailsDto
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.UserProfile != null ? u.UserProfile.FullName : u.UserName,
                Email = u.Email,
                ProfilePictureUrl = u.UserProfile != null ? u.UserProfile.ProfilePictureUrl : null,
                Gender = u.UserProfile != null ? u.UserProfile.Gender : null,
                PhoneNumber = u.UserProfile != null ? u.UserProfile.PhoneNumber : null,
                DateOfBirth = u.UserProfile != null ? u.UserProfile.DateOfBirth : null,
                IsActive = !u.IsDeleted,
                LastSuccessfulLoginAt = u.LastSuccessfulLoginAt,
                PasswordChangedAt = u.PasswordChangedAt,
                Role = new RoleDto
                {
                    Id = u.RoleId,
                    Name = u.Role.Name,
                    DisplayName = u.Role.DisplayName
                }
            })
            .FirstOrDefaultAsync(ctn);

        return user;
    }

    public async Task<UserDetailsDto?> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto dto, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var user = await _dbContext.Set<User>()
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ctn);

        if (user == null)
            return null;

        user.UpdateEmail(dto.Email ?? user.Email);

        if (user.UserProfile != null)
        {
            user.UserProfile.UpdateFullName(dto.FullName ?? user.UserProfile.FullName);
            user.UserProfile.UpdatePhoneNumber(dto.PhoneNumber ?? user.UserProfile.PhoneNumber);
            user.UserProfile.UpdateGender(dto.Gender ?? user.UserProfile.Gender);
            user.UserProfile.UpdateDateOfBirth(dto.DateOfBirth ?? user.UserProfile.DateOfBirth);
        }

        await _dbContext.SaveChangesAsync(ctn);

        var userProfile = new UserDetailsDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.UserProfile != null ? user.UserProfile.FullName : user.UserName,
            Email = user.Email,
            ProfilePictureUrl = user.UserProfile != null ? user.UserProfile.ProfilePictureUrl : null,
            Gender = user.UserProfile != null ? user.UserProfile.Gender : null,
            PhoneNumber = user.UserProfile != null ? user.UserProfile.PhoneNumber : null,
            DateOfBirth = user.UserProfile != null ? user.UserProfile.DateOfBirth : null,
            IsActive = !user.IsDeleted,
            LastSuccessfulLoginAt = user.LastSuccessfulLoginAt,
            PasswordChangedAt = user.PasswordChangedAt,
            Role = new RoleDto
            {
                Id = user.RoleId,
                Name = user.Role.Name,
                DisplayName = user.Role.DisplayName
            }
        };
        return userProfile;
    }

    public async Task<UserDetailsDto?> UpdateProfilePictureAsync(Guid userId, string pictureUrl, CancellationToken ctn)
    {
        var user = await _dbContext.Set<User>()
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ctn);

        if (user == null)
            return null;

        if (user.UserProfile is null)
            user.SetProfile(UserProfile.Create(user.Id, user.UserName));

        user.UserProfile?.UpdateProfilePicture(pictureUrl);
        await _dbContext.SaveChangesAsync(ctn);

        var userProfile = new UserDetailsDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.UserProfile != null ? user.UserProfile.FullName : user.UserName,
            Email = user.Email,
            ProfilePictureUrl = user.UserProfile != null ? user.UserProfile.ProfilePictureUrl : null,
            Gender = user.UserProfile != null ? user.UserProfile.Gender : null,
            PhoneNumber = user.UserProfile != null ? user.UserProfile.PhoneNumber : null,
            DateOfBirth = user.UserProfile != null ? user.UserProfile.DateOfBirth : null,
            IsActive = !user.IsDeleted,
            LastSuccessfulLoginAt = user.LastSuccessfulLoginAt,
            PasswordChangedAt = user.PasswordChangedAt,
            Role = new RoleDto
            {
                Id = user.RoleId,
                Name = user.Role.Name,
                DisplayName = user.Role.DisplayName
            }
        };
        return userProfile;
    }

    public async Task ClearProfilePictureAsync(Guid userId, CancellationToken ctn)
    {
        var user = await _dbContext.Set<User>()
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ctn);

        if (user?.UserProfile != null)
        {
            user.UserProfile.ClearProfilePicture();
            await _dbContext.SaveChangesAsync(ctn);
        }
    }
}
