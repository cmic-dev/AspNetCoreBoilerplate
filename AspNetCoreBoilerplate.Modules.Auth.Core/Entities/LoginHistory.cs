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

    public static LoginHistory Create(User user, string? ipAddress, string? userAgent, string? device, string? platform, string? browser)
    {
        var loginHistory = new LoginHistory
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Device = device,
            Platform = platform,
            Browser = browser
        };
        return loginHistory;
    }
}
