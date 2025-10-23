using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string message = "User not found")
        : base(message, StatusCodes.Status404NotFound)
    {
    }
}
