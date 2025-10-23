using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Shared.Exceptions;

public class DomainException : Exception
{
    public int StatusCode { get; }

    public DomainException(string message, int statusCode = StatusCodes.Status400BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public DomainException(string message, Exception innerException, int statusCode = StatusCodes.Status400BadRequest)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
