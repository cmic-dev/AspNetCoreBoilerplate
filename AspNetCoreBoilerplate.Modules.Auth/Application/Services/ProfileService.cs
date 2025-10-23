using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Modules.Auth.Application.Exceptions;
using AspNetCoreBoilerplate.Modules.Auth.Application.Providers;
using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Services;

public class ProfileService
{
    private readonly IAppDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly PasswordProvider _passwordProvider;


    public ProfileService(IAppDbContext dbContext, IUserContext userContext, PasswordProvider passwordProvider)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _passwordProvider = passwordProvider;
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(Guid userId, CancellationToken ctn = default)
    {
        var user = await _dbContext.Set<User>()
            .Include(x => x.Role)
            .Where(x => x.Id == userId && !x.IsDeleted)
            .Select(x => new UserDetailsDto
            {
                Id = x.Id,
                UserName = x.UserName,
                DisplayName = x.DisplayName,
                Email = x.Email,
                IsActive = x.IsDeleted,
                LastSuccessfulLoginAt = x.LastSuccessfulLoginAt,
                PasswordChangedAt = x.PasswordChangedAt,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                PhoneNumber = x.PhoneNumber,
                ProfilePictureUrl = x.ProfilePictureUrl,
                Role = new RoleDto
                {
                    Id = x.Role.Id,
                    Name = x.Role.Name,
                    DisplayName = x.Role.DisplayName
                }
            })
            .FirstOrDefaultAsync(ctn);

        return user;
    }


    public async Task<UserDetailsDto> UpdateProfileAsync(Guid userId, UpdateProfileRequestDto request, CancellationToken ctn)
    {
        var currentUser = await _dbContext.Set<User>()
            .Where(x => x.Id == userId)
            .Include(x => x.Role)
            .FirstOrDefaultAsync(ctn);

        if (currentUser is null)
            throw new UserNotFoundException("User not found");

        currentUser.UpdateProfile(
            request.DisplayName,
            request.Gender,
            request.PhoneNumber,
            request.DateOfBirth);

        await _dbContext.SaveChangesAsync(ctn);

        var userDetails = new UserDetailsDto
        {
            Id = currentUser.Id,
            UserName = currentUser.UserName,
            DisplayName = currentUser.DisplayName,
            Email = currentUser.Email,
            IsActive = currentUser.IsDeleted,
            LastSuccessfulLoginAt = currentUser.LastSuccessfulLoginAt,
            PasswordChangedAt = currentUser.PasswordChangedAt,
            DateOfBirth = currentUser.DateOfBirth,
            Gender = currentUser.Gender,
            PhoneNumber = currentUser.PhoneNumber,
            ProfilePictureUrl = currentUser.ProfilePictureUrl,
            Role = new RoleDto
            {
                Id = currentUser.Role.Id,
                Name = currentUser.Role.Name,
                DisplayName = currentUser.Role.DisplayName
            }
        };
        return userDetails;
    }

    public async Task<UserDetailsDto> UpdateEmailAddressAsync(Guid userId, UpdateEmailRequestDto request, CancellationToken ctn)
    {
        var currentUser = await _dbContext.Set<User>()
            .Where(x => x.Id == userId)
            .Include(x => x.Role)
            .FirstOrDefaultAsync(ctn);

        if (currentUser is null)
            throw new UserNotFoundException("User not found");

        if (!_passwordProvider.VerifyPassword(request.Password, currentUser.Password))
            throw new InvalidPasswordException("Current password is incorrect");

        currentUser.UpdateEmail(request.NewEmail);

        await _dbContext.SaveChangesAsync(ctn);

        var userDetails = new UserDetailsDto
        {
            Id = currentUser.Id,
            UserName = currentUser.UserName,
            DisplayName = currentUser.DisplayName,
            Email = currentUser.Email,
            IsActive = currentUser.IsDeleted,
            LastSuccessfulLoginAt = currentUser.LastSuccessfulLoginAt,
            PasswordChangedAt = currentUser.PasswordChangedAt,
            DateOfBirth = currentUser.DateOfBirth,
            Gender = currentUser.Gender,
            PhoneNumber = currentUser.PhoneNumber,
            ProfilePictureUrl = currentUser.ProfilePictureUrl,
            Role = new RoleDto
            {
                Id = currentUser.Role.Id,
                Name = currentUser.Role.Name,
                DisplayName = currentUser.Role.DisplayName
            }
        };
        return userDetails;
    }
}
