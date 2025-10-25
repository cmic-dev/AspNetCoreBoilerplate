using AspNetCoreBoilerplate.Shared.Entities;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Entities;

public class User : SoftDeletableEntity<Guid>
{
    public static readonly User System = new User()
    {
        Id = Guid.Parse("0a622c44-e9a4-414e-b8af-44d70c90f0b3"),
        UserName = "system",
        IsSystem = true,
        Password = "sD3fPKLnFKZUjnSV4qA/XoJOqsmDfNfxWcZ7kPtLc0I=",
        RoleId = Role.SuperAdmin.Id,
    };

    public static readonly User SuperAdmin = new User()
    {
        Id = Guid.Parse("b348eeb7-f5b7-4076-9a57-168f9052c342"),
        UserName = "admin",
        IsSystem = true,
        Password = "sD3fPKLnFKZUjnSV4qA/XoJOqsmDfNfxWcZ7kPtLc0I=", // P@ssw0rd
        RoleId = Role.SuperAdmin.Id
    };

    private readonly List<RefreshToken> _refreshTokens = new();

    private User() { }

    public string UserName { get; private set; } = string.Empty;
    public string? Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;

    public int FailedLoginAttempts { get; private set; }
    public DateTime? LastFailedLoginAt { get; private set; }

    public DateTime? LastSuccessfulLoginAt { get; private set; }

    public DateTime? PasswordChangedAt { get; private set; }
    public bool RequiresPasswordChange { get; private set; }

    public UserProfile? UserProfile { get; private set; } = null!;

    public static User Create(string userName, string? email, string hashedPassword, Guid roleId, bool requiresPasswordChange = false)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty", nameof(userName));

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName.Trim(),
            Email = email?.Trim(),
            Password = hashedPassword,
            RoleId = roleId,
            RequiresPasswordChange = requiresPasswordChange,
            PasswordChangedAt = requiresPasswordChange ? null : DateTime.UtcNow
        };

        user.RecordDomainEvent(new Events.UserCreatedEvent
        {
            UserId = user.Id,
            UserName = user.UserName
        });
        return user;
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        LastFailedLoginAt = DateTime.UtcNow;
    }

    public bool IsTempLockedOut()
    {
        if (FailedLoginAttempts < 5 || !LastFailedLoginAt.HasValue)
            return false;

        TimeSpan timeSinceLastFailure = DateTime.UtcNow - LastFailedLoginAt.Value;
        int lockoutDurationMinutes = 5;

        return timeSinceLastFailure.TotalMinutes < lockoutDurationMinutes;
    }

    public void UnLock()
    {
        FailedLoginAttempts = 0;
        LastFailedLoginAt = null;
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LastFailedLoginAt = null;
        LastSuccessfulLoginAt = DateTime.UtcNow;
    }

    public bool IsSystem { get; private set; } = false;
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public void RemoveExpiredRefreshTokens()
    {
        var expiredTokens = _refreshTokens.Where(rt => rt.IsExpired()).ToList();
        foreach (var token in expiredTokens)
        {
            token.Revoke("Expired");
        }
    }

    public void RevokeAllRefreshTokens(string? reason = null)
    {
        foreach (var token in _refreshTokens.Where(rt => rt.IsRevoked))
        {
            token.Revoke(reason ?? "User logout or security revocation");
        }
    }

    public void UpdateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty", nameof(userName));

        UserName = userName.Trim();
    }

    public void UpdateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("email cannot be empty", nameof(email));

        Email = email?.Trim();
    }

    public void UpdatePassword(string hashedPassword)
    {
        Password = hashedPassword;
        PasswordChangedAt = DateTime.UtcNow;
        RequiresPasswordChange = false;
    }

    public void SetProfile(UserProfile userProfile)
    {
        UserProfile = userProfile;
    }
}
