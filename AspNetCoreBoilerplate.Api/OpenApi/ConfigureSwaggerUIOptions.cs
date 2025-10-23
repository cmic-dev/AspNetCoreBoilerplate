using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AspNetCoreBoilerplate.Api.OpenApi;

public class ConfigureSwaggerUIOptions(IApiVersionDescriptionProvider apiVersionDescription) : IConfigureNamedOptions<SwaggerUIOptions>
{
    public void Configure(SwaggerUIOptions options)
    {
        foreach (var description in apiVersionDescription.ApiVersionDescriptions)
        {
            string url = $"/swagger/{description.GroupName}/swagger.json";
            string name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    }

    public void Configure(string? name, SwaggerUIOptions options) => Configure(options);
}

