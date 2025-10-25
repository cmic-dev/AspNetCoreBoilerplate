using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Exceptions;

public class PasswordChangeRequiredException : DomainException
{
    public PasswordChangeRequiredException(string message = "Password change required")
        : base(message, StatusCodes.Status403Forbidden)
    {
    }
}
