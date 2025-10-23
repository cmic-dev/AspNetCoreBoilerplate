using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AspNetCoreBoilerplate.Core.ExceptionHandlers;

internal sealed class RateLimitExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.StatusCode != StatusCodes.Status429TooManyRequests)
        {
            return false;
        }

        httpContext.Response.ContentType = "application/problem+json";

        var retryAfter = TimeSpan.FromSeconds(60); // Default
        if (httpContext.Response.Headers.TryGetValue("Retry-After", out var retryAfterHeader))
        {
            if (int.TryParse(retryAfterHeader, out var seconds))
            {
                retryAfter = TimeSpan.FromSeconds(seconds);
            }
        }

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc6585#section-4",
            Title = "Too Many Requests",
            Status = StatusCodes.Status429TooManyRequests,
            Detail = $"Rate limit exceeded. Please retry after {FormatRetryAfter(retryAfter)}.",
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        problemDetails.Extensions["retryAfter"] = (int)retryAfter.TotalSeconds;
        problemDetails.Extensions["retryAfterFormatted"] = FormatRetryAfter(retryAfter);
        problemDetails.Extensions["limit"] = GetRateLimitInfo(httpContext);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static string FormatRetryAfter(TimeSpan retryAfter)
    {
        if (retryAfter.TotalSeconds < 60)
            return $"{(int)retryAfter.TotalSeconds} second(s)";

        if (retryAfter.TotalMinutes < 60)
            return $"{(int)retryAfter.TotalMinutes} minute(s)";

        return $"{(int)retryAfter.TotalHours} hour(s)";
    }

    private static object GetRateLimitInfo(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        var rateLimitMetadata = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();

        return new
        {
            policy = rateLimitMetadata?.PolicyName ?? "default",
            message = "Rate limit varies based on authentication status"
        };
    }
}
