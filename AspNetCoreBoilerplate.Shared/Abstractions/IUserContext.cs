namespace AspNetCoreBoilerplate.Shared.Abstractions;

public interface IUserContext
{
    Guid? UserId { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
    string? Device { get; }
    string? Platform { get; }
    string? Browser { get; }
    string? UserName { get; }
}
