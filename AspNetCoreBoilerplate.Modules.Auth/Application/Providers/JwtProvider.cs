using AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Providers;

public class GenerateTokenResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}

public class JwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtProvider(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions?.Value ??
            throw new ArgumentNullException(
                nameof(jwtOptions),
                "JWT options cannot be null");

        // Validate and cache the signing key
        _signingKey = CreateAndValidateSigningKey();
    }

    public GenerateTokenResult GenerateToken(IReadOnlyList<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var credentials = new SigningCredentials(
            _signingKey,
            SecurityAlgorithms.HmacSha256);

        var tokenExpires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer,
            SigningCredentials = credentials,
            Expires = tokenExpires,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new GenerateTokenResult
        {
            Token = tokenHandler.WriteToken(token),
            Expiration = tokenExpires
        };
    }

    public GenerateTokenResult GenerateRefreshToken()
    {
        Span<byte> randomNumber = stackalloc byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var refreshToken = Convert.ToBase64String(randomNumber)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return new GenerateTokenResult
        {
            Token = refreshToken,
            Expiration = DateTime.UtcNow.AddDays(7)
        };
    }

    private SymmetricSecurityKey CreateAndValidateSigningKey()
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
        {
            throw new InvalidOperationException("JWT signing key cannot be null or empty");
        }

        var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key must be at least 32 bytes (256 bits) for HMAC-SHA256. " +
                $"Current key length: {keyBytes.Length} bytes");
        }

        return new SymmetricSecurityKey(keyBytes);
    }
}
