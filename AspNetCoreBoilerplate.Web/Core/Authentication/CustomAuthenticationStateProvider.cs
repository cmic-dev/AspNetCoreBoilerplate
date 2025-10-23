using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AspNetCoreBoilerplate.Web.Core.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private UserDetailsDto? _currentAuth;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_currentAuth == null)
            return Task.FromResult(new AuthenticationState(_anonymous));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _currentAuth.Id.ToString()),
            new Claim(ClaimTypes.Name, _currentAuth.UserName),
            new Claim(ClaimTypes.GivenName, _currentAuth.DisplayName ?? ""),
            new Claim(ClaimTypes.Surname, _currentAuth.DisplayName ?? ""),
            new Claim(ClaimTypes.Email, _currentAuth.Email ?? ""),
            new Claim(ClaimTypes.Role, _currentAuth.Role.Name),
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(user));
    }

    public void NotifyAuthChanged(UserDetailsDto? authResponseDto)
    {
        _currentAuth = authResponseDto;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void MarkUserAsLoggedOut()
    {
        _currentAuth = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
