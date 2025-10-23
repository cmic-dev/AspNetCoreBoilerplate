using AspNetCoreBoilerplate.Shared.RateLimiter;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace AspNetCoreBoilerplate.Api.RateLimiter;

public class ConfigureRateLimiterOptions : IConfigureOptions<RateLimiterOptions>
{
    public void Configure(RateLimiterOptions options)
    {
        options.AddFixedWindowLimiter(RateLimiterPolicies.Anonymous, opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 10;
            opt.QueueLimit = 0;
        });

        options.AddFixedWindowLimiter(RateLimiterPolicies.Authenticated, opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 100;
            opt.QueueLimit = 0;
        });

        options.AddFixedWindowLimiter(RateLimiterPolicies.Auth, opt =>
        {
            opt.Window = TimeSpan.FromMinutes(1);
            opt.PermitLimit = 5;
            opt.QueueLimit = 0;
        });

        options.OnRejected = static async (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/problem+json";

            var problemDetailsService = context.HttpContext.RequestServices
                .GetRequiredService<IProblemDetailsService>();

            var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
                ? retryAfterValue
                : TimeSpan.FromSeconds(60);

            // Add Retry-After header
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc6585#section-4",
                Title = "Too Many Requests",
                Status = StatusCodes.Status429TooManyRequests,
                Detail = "Rate limit exceeded. Please try again later.",
                Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}"
            };

            // Add rate limit specific information
            problemDetails.Extensions["retryAfter"] = (int)retryAfter.TotalSeconds;
            problemDetails.Extensions["retryAfterFormatted"] = FormatRetryAfter(retryAfter);
            problemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;

            // Add correlation ID if present
            if (context.HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                problemDetails.Extensions["correlationId"] = correlationId.ToString();
            }

            // Add limit information in development
            var environment = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
            if (environment?.IsDevelopment() ?? false)
            {
                var endpoint = context.HttpContext.GetEndpoint();
                var rateLimitPolicy = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>();

                problemDetails.Extensions["rateLimitPolicy"] = rateLimitPolicy?.PolicyName ?? "unknown";
            }

            await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context.HttpContext,
                ProblemDetails = problemDetails
            });
        };
    }

    static string FormatRetryAfter(TimeSpan retryAfter)
    {
        if (retryAfter.TotalSeconds < 60)
            return $"{(int)retryAfter.TotalSeconds} seconds";

        if (retryAfter.TotalMinutes < 60)
            return $"{(int)retryAfter.TotalMinutes} minutes";

        return $"{(int)retryAfter.TotalHours} hours";
    }
}
