using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Jwt;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    [MinLength(32, ErrorMessage = "JWT key must be at least 32 characters long for security")]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(5, 1440, ErrorMessage = "Access token expiration must be between 5 minutes and 24 hours")]
    public int AccessTokenExpirationMinutes { get; set; } = 60;
}
