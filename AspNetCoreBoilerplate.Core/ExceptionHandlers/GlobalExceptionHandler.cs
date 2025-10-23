using AspNetCoreBoilerplate.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreBoilerplate.Core.ExceptionHandlers;

internal sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode = exception is DomainException domainException
            ? domainException.StatusCode
            : StatusCodes.Status500InternalServerError;

        httpContext.Response.StatusCode = statusCode;

        ProblemDetails problemDetails = new ProblemDetails()
        {
            Type = exception.GetType().Name,
            Title = "An error occurred",
            Detail = exception.Message,
            Status = statusCode
        };

        // Add correlation ID if present
        if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId.ToString();
        }

        // Add exception details in development only
        if (ShouldIncludeExceptionDetails(httpContext) && exception != null)
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    Type = exception.InnerException.GetType().Name,
                    exception.InnerException.Message
                };
            }
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static bool ShouldIncludeExceptionDetails(HttpContext httpContext)
    {
        var environment = httpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>();
        return environment.IsDevelopment();
    }
}
