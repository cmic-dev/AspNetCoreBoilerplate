using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Modules.Auth.Core.Exceptions;
using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Services;

internal class AuthService : IAuthService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordProvider _passwordProvider;
    private readonly IAppDbContext _dbContext;
    private readonly IUserContext _userContext;

    public AuthService(IJwtProvider jwtProvider, IPasswordProvider passwordProvider, IAppDbContext dbContext, IUserContext userContext)
    {
        _jwtProvider = jwtProvider;
        _passwordProvider = passwordProvider;
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ctn = default)
    {
        var ipAddress = _userContext.IpAddress ?? "Unknown";
        var userAgent = _userContext.UserAgent;
        var device = _userContext.Device;
        var platform = _userContext.Platform;
        var browser = _userContext.Browser;

        // Find user
        var user = await _dbContext.Set<User>()
            .Where(x => x.UserName == request.UserName)
            .Include(x => x.Role)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(ctn);

        // Check account lockout
        if (user != null && user.IsTempLockedOut())
            throw new AccountLockedException("Account is temporarily locked due to multiple failed login attempts. Please try again later.");

        if (user is null || !_passwordProvider.VerifyPassword(request.Password, user.Password))
        {
            if (user != null)
            {
                user.RecordFailedLogin();
                await _dbContext.SaveChangesAsync(ctn);
            }
            throw new InvalidCredentialsException();
        }

        if (user.IsDeleted)
            throw new AccountDisabledException("Account is disabled");

        if (user.RequiresPasswordChange)
            throw new PasswordChangeRequiredException();

        // Generate tokens
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        var tokenResult = _jwtProvider.GenerateToken(claims);
        var refreshTokenResult = _jwtProvider.GenerateRefreshToken();

        using var transaction = await _dbContext.Database.BeginTransactionAsync(ctn);
        try
        {
            // Create refresh token entity
            var newToken = RefreshToken.Create(
                user.Id,
                refreshTokenResult.Token,
                refreshTokenResult.Expiration,
                userAgent,
                ipAddress
            );
            user.AddRefreshToken(newToken);
            user.RecordSuccessfulLogin();

            // Log successful login
            _dbContext.Set<LoginHistory>().Add(LoginHistory.Create(
                user,
                ipAddress,
                userAgent));

            await _dbContext.SaveChangesAsync(ctn);
            await transaction.CommitAsync(ctn);
        }
        catch
        {
            await transaction.RollbackAsync(ctn);
            throw;
        }

        return new AuthResponseDto
        {
            AccessToken = tokenResult.Token,
            TokenExpiry = tokenResult.Expiration,
            RefreshToken = refreshTokenResult.Token,
            UserId = user.Id
        };
    }

    public async Task LogoutAsync(LogoutRequestDto request, CancellationToken ctn = default)
    {
        var ipAddress = _userContext.IpAddress ?? "Unknown";
        var userName = _userContext.UserName ?? "Unknown";

        var token = await _dbContext.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, ctn);

        if (token == null)
            throw new TokenNotFoundException("Token not found");

        token.Revoke("User logout");
        await _dbContext.SaveChangesAsync(ctn);
    }

    public async Task LogoutAllDevicesAsync(Guid userId, CancellationToken ctn = default)
    {
        var ipAddress = _userContext.IpAddress ?? "Unknown";

        var user = await _dbContext.Set<User>()
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == userId, ctn);

        if (user is null)
            throw new UserNotFoundException("User not found");

        user.RevokeAllRefreshTokens("User logout");
        await _dbContext.SaveChangesAsync(ctn);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken ctn = default)
    {
        var ipAddress = _userContext.IpAddress ?? "Unknown";

        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(x => x.Id == userId, ctn);

        if (user is null)
            throw new UserNotFoundException("User not found");

        // Verify current password
        if (!_passwordProvider.VerifyPassword(request.CurrentPassword, user.Password))
            throw new InvalidPasswordException("Current password is incorrect");

        // Check if new password is same as current
        if (request.NewPassword == request.CurrentPassword)
            throw new InvalidPasswordException("New password must be different from current password");

        using var transaction = await _dbContext.Database.BeginTransactionAsync(ctn);
        try
        {
            // Hash new password
            user.UpdatePassword(_passwordProvider.HashPassword(request.NewPassword));

            // Revoke all refresh tokens for security
            user.RevokeAllRefreshTokens("Password changed");

            await _dbContext.SaveChangesAsync(ctn);
            await transaction.CommitAsync(ctn);
        }
        catch
        {
            await transaction.RollbackAsync(ctn);
            throw;
        }
    }

    public async Task<IEnumerable<LoginHistoryDto>> GetLoginHistoriesAsync(GetLoginHistoriesRequestDto request, CancellationToken ctn = default)
    {
        var query = _dbContext.Set<LoginHistory>().AsQueryable();

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.Timestamp >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.Timestamp <= request.ToDate.Value);

        var logs = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new LoginHistoryDto
            {
                Id = x.Id,
                UserName = x.User.UserName,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                Device = x.Device,
                Platform = x.Platform,
                Browser = x.Browser,
                Timestamp = x.Timestamp
            })
            .ToListAsync(ctn);

        return logs;
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ctn = default)
    {
        var ipAddress = _userContext.IpAddress ?? "Unknown";
        var userAgent = _userContext.UserAgent;
        var device = _userContext.Device;
        var platform = _userContext.Platform;
        var browser = _userContext.Browser;

        var token = await _dbContext.Set<RefreshToken>()
            .Include(rt => rt.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt =>
                rt.Token == request.RefreshToken &&
                rt.IsRevoked &&
                rt.ExpiryTime > DateTime.UtcNow, ctn);

        if (token == null)
            throw new InvalidRefreshTokenException("Invalid or expired refresh token");

        var user = token.User;

        if (user.IsDeleted)
            throw new AccountDisabledException("Account is disabled");

        // Generate new tokens
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var tokenResult = _jwtProvider.GenerateToken(claims);
        var newRefreshTokenResult = _jwtProvider.GenerateRefreshToken();

        using var transaction = await _dbContext.Database.BeginTransactionAsync(ctn);
        try
        {
            // Update last accessed time
            token.UpdateLastAccessed();

            // Create new refresh token
            var newToken = RefreshToken.Create(
                user.Id,
                newRefreshTokenResult.Token,
                newRefreshTokenResult.Expiration,
                userAgent,
                ipAddress
            );

            // Revoke old token and add new one
            token.Revoke("Replaced with new token");
            user.AddRefreshToken(newToken);

            await _dbContext.SaveChangesAsync(ctn);
            await transaction.CommitAsync(ctn);
        }
        catch
        {
            await transaction.RollbackAsync(ctn);
            throw;
        }

        return new AuthResponseDto
        {
            AccessToken = tokenResult.Token,
            TokenExpiry = tokenResult.Expiration,
            RefreshToken = newRefreshTokenResult.Token,
            UserId = user.Id
        };
    }

    public async Task RevokeTokenAsync(RevokeTokenRequestDto request, CancellationToken ctn = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new ArgumentException("Refresh token must be provided", nameof(request.RefreshToken));

        var ipAddress = _userContext.IpAddress ?? "Unknown";

        var token = await _dbContext.Set<RefreshToken>()
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken.Trim(), ctn);

        if (token == null || token.UserId != _userContext.UserId)
            throw new TokenNotFoundException("Token not found");

        token.Revoke("User requested revocation");

        await _dbContext.SaveChangesAsync(ctn);
    }

    public async Task<IEnumerable<ActiveSessionDto>> GetActiveSessionsAsync(Guid userId, CancellationToken ctn = default)
    {
        var sessions = await _dbContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && rt.IsRevoked && rt.ExpiryTime > DateTime.UtcNow)
            .OrderByDescending(rt => rt.LastAccessedAt ?? rt.CreatedAt)
            .Select(rt => new ActiveSessionDto
            {
                Id = rt.Id,
                Device = rt.Device,
                Platform = rt.Platform,
                Browser = rt.Browser,
                IpAddress = rt.IpAddress,
                CreatedAt = rt.CreatedAt,
                LastAccessedAt = rt.LastAccessedAt,
                IsCurrent = rt.LastAccessedAt != null && rt.LastAccessedAt > DateTime.UtcNow.AddMinutes(-5)
            })
            .ToListAsync(ctn);

        return sessions;
    }
}
