using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Exceptions;

public class InvalidRefreshTokenException : DomainException
{
    public InvalidRefreshTokenException(string message = "Invalid or expired refresh token")
        : base(message, StatusCodes.Status401Unauthorized)
    {
    }
}
