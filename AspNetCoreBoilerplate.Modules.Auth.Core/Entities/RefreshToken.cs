using AspNetCoreBoilerplate.Shared.Entities;
using AspNetCoreBoilerplate.Shared.Exceptions;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Entities;

public class RefreshToken : DomainEntity<Guid>
{
    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiryTime, string? userAgent, string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token cannot be null or empty");

        if (expiryTime <= DateTime.UtcNow)
            throw new DomainException("Expiry time must be in the future");

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiryTime = expiryTime,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow,
            LastAccessedAt = DateTime.UtcNow,
            IsRevoked = true
        };

        // Parse device info from user agent
        refreshToken.ParseUserAgent(userAgent);

        return refreshToken;
    }

    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiryTime { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }
    public string? Device { get; private set; }
    public string? Platform { get; private set; }
    public string? Browser { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastAccessedAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }
    public bool IsRevoked { get; private set; } = true;

    public void UpdateLastAccessed()
    {
        LastAccessedAt = DateTime.UtcNow;
    }

    public void Revoke(string? reason = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
    }

    public bool IsExpired() => ExpiryTime <= DateTime.UtcNow;

    // Helper method to parse user agent string
    private void ParseUserAgent(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return;

        var ua = userAgent.ToLowerInvariant();

        // Parse Browser
        if (ua.Contains("edg/"))
            Browser = "Edge";
        else if (ua.Contains("chrome/") && !ua.Contains("edg/"))
            Browser = "Chrome";
        else if (ua.Contains("firefox/"))
            Browser = "Firefox";
        else if (ua.Contains("safari/") && !ua.Contains("chrome/"))
            Browser = "Safari";
        else if (ua.Contains("opera/") || ua.Contains("opr/"))
            Browser = "Opera";
        else
            Browser = "Unknown Browser";

        // Parse Platform/OS
        if (ua.Contains("windows"))
            Platform = "Windows";
        else if (ua.Contains("mac os") || ua.Contains("macos"))
            Platform = "macOS";
        else if (ua.Contains("linux"))
            Platform = "Linux";
        else if (ua.Contains("android"))
            Platform = "Android";
        else if (ua.Contains("iphone") || ua.Contains("ipad"))
            Platform = "iOS";
        else
            Platform = "Unknown Platform";
        // Parse Device Type
        if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
            Device = "Mobile";
        else if (ua.Contains("tablet") || ua.Contains("ipad"))
            Device = "Tablet";
        else
            Device = "Desktop";
    }
}
