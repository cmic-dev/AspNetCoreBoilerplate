namespace AspNetCoreBoilerplate.Modules.Auth.Application.Models;

public class GenerateTokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}
