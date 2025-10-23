using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace AspNetCoreBoilerplate.Api.ProblemDetails;

public class ConfigureProblemDetailsOptions : IConfigureOptions<ProblemDetailsOptions>
{
    public void Configure(ProblemDetailsOptions options)
    {
        options.CustomizeProblemDetails = ctx =>
        {
            ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";

            // Add tracing info
            var activity = ctx.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            ctx.ProblemDetails.Extensions = new Dictionary<string, object?>()
            {
                ["requestId"] = ctx.HttpContext.TraceIdentifier,
                ["timestamp"] = DateTimeOffset.UtcNow,
                ["traceId"] = activity?.Id,
            };
        };
    }
}
