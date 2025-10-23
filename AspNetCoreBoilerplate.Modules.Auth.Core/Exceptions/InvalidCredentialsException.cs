using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Exceptions;

public class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException(string message = "Invalid username or password")
        : base(message, StatusCodes.Status401Unauthorized)
    {
    }
}