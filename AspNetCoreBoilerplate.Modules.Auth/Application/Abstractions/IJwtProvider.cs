using AspNetCoreBoilerplate.Modules.Auth.Application.Models;
using System.Security.Claims;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;

public interface IJwtProvider
{
    GenerateTokenResult GenerateToken(IReadOnlyList<Claim> claims);
    GenerateTokenResult GenerateRefreshToken();
}
