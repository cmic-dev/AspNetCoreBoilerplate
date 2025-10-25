using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Exceptions;

public class InvalidPasswordException : DomainException
{
    public InvalidPasswordException(string message)
        : base(message, StatusCodes.Status400BadRequest)
    {
    }
}