using System.Security.Claims;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class ClaimsPrincipalExceptions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new Exception("User ID claim is missing or invalid.");
        }
        return userId;
    }

    public static string GetUserName(this ClaimsPrincipal user)
    {
        var userNameClaim = user.FindFirst(ClaimTypes.Name);
        if (userNameClaim == null)
        {
            throw new Exception("User Name claim is missing.");
        }
        return userNameClaim.Value;
    }

    public static string GetDisplayName(this ClaimsPrincipal user)
    {
        var displayNameClaim = user.FindFirst(ClaimTypes.Surname);
        if (displayNameClaim == null)
        {
            throw new Exception("Display Name claim is missing.");
        }
        return displayNameClaim.Value;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst(ClaimTypes.Email);
        if (emailClaim == null)
        {
            throw new Exception("Email claim is missing.");
        }
        return emailClaim.Value;
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        var roleClaim = user.FindFirst(ClaimTypes.Role);
        if (roleClaim == null)
        {
            throw new Exception("Role claim is missing.");
        }
        return roleClaim.Value;
    }
}
