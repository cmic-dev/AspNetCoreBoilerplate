namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class ActiveSessionDto
{
    public Guid Id { get; set; }
    public string? Device { get; set; }
    public string? Platform { get; set; }
    public string? Browser { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public bool IsCurrent { get; set; }
}
