using AspNetCoreBoilerplate.Shared.Entities;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Entities;

public class LoginHistory : DomainEntity<Guid>
{
    private LoginHistory() { }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    public string? Device { get; private set; }
    public string? Platform { get; private set; }
    public string? Browser { get; private set; }

    public DateTime Timestamp { get; private set; }

    public static LoginHistory Create(User user, string? ipAddress, string? userAgent)
    {
        var loginHistory = new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Timestamp = DateTime.UtcNow
        };
        loginHistory.ParseUserAgent(userAgent);
        return loginHistory;
    }

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
