namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class LoginHistoryDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public string? Device { get; set; }
    public string? Platform { get; set; }
    public string? Browser { get; set; }

    public DateTime Timestamp { get; set; }
}
