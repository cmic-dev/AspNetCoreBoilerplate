using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Exceptions;

public class TokenNotFoundException : DomainException
{
    public TokenNotFoundException(string message = "Token not found")
        : base(message, StatusCodes.Status404NotFound)
    {
    }
}
