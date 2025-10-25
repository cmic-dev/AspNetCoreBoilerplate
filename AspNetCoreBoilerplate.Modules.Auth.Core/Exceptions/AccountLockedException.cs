using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Exceptions;

public class AccountLockedException : DomainException
{
    public AccountLockedException(string message = "Account is temporarily locked due to multiple failed login attempts")
        : base(message, StatusCodes.Status423Locked)
    {
    }
}
