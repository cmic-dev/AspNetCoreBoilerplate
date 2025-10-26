using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Services;

internal class UserService : IUserService
{
    private readonly IAppDbContext _dbContext;
    private readonly IPasswordProvider _passwordProvider;
    public UserService(IAppDbContext dbContext, IPasswordProvider passwordProvider)
    {
        _dbContext = dbContext;
        _passwordProvider = passwordProvider;
    }

    public async Task<IEnumerable<UserDetailsDto>> GetAllActiveUsersAsync(CancellationToken ctn = default)
    {
        return await _dbContext.Set<User>()
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
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
            .ToListAsync(ctn);
    }

    public async Task<Guid> CreateUserAsync(CreateUserRequestDto dto, CancellationToken ctn = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var role = await _dbContext.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == dto.RoleId, ctn);

        if (role == null)
            throw new DomainException("Invalid role ID");

        var hashedPassword = _passwordProvider.HashPassword(dto.Password);

        var user = User.Create(
            dto.UserName,
            dto.Email,
            hashedPassword,
            dto.RoleId);

        _dbContext.Set<User>().Add(user);
        await _dbContext.SaveChangesAsync(ctn);
        return user.Id;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ctn);

        if (user == null)
            return false;

        _dbContext.Set<User>().Remove(user);
        await _dbContext.SaveChangesAsync(ctn);
        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid userId, CancellationToken ctn = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        var user = await _dbContext.Set<User>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsDeleted, ctn);

        if (user == null)
            return false;

        user.Restore();

        await _dbContext.SaveChangesAsync(ctn);
        return true;
    }
}
