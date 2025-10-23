using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Exceptions;

public class AccountDisabledException : DomainException
{
    public AccountDisabledException(string message = "Account is disabled")
        : base(message, StatusCodes.Status403Forbidden)
    {
    }
}
