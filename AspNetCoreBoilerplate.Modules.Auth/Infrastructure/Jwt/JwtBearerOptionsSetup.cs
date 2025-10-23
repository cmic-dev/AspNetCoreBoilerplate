using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Jwt;

public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtBearerOptionsSetup> _logger;

    public JwtBearerOptionsSetup(
        IOptions<JwtOptions> jwtOptions,
        ILogger<JwtBearerOptionsSetup> logger)
    {
        _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Configure(JwtBearerOptions options)
    {
        var signingKey = CreateSigningKey();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Issuer validation
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,

            // Audience validation
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,

            // Signing key validation
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            // Lifetime validation
            ValidateLifetime = true,
            RequireExpirationTime = true,

            // Security enhancements
            RequireSignedTokens = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },

            // Name and role claim mapping
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };

        // Event handlers for better logging and debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                _logger.LogWarning(
                    "JWT authentication failed: {Error}",
                    context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                _logger.LogDebug(
                    "JWT token validated successfully for user: {User}",
                    context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                _logger.LogDebug(
                    "JWT authentication challenge triggered: {Error}",
                    context.Error ?? "No error specified");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                _logger.LogDebug(
                    "JWT authorization forbidden for user: {User}",
                    context.Principal?.Identity?.Name ?? "Unknown");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    _logger.LogDebug("JWT token received");
                }
                return Task.CompletedTask;
            }
        };
    }

    public void Configure(string? name, JwtBearerOptions options) => Configure(options);

    private SymmetricSecurityKey CreateSigningKey()
    {
        try
        {
            var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);

            // ADDED: Validate key length
            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT signing key must be at least 32 bytes (256 bits) for HMAC-SHA256");
            }

            return new SymmetricSecurityKey(keyBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create JWT signing key");
            throw new InvalidOperationException("Failed to create JWT signing key", ex);
        }
    }
}
