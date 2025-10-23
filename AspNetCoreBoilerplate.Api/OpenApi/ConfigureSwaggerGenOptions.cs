using AspNetCoreBoilerplate.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCoreBoilerplate.Api.OpenApi;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescription, IWebHostEnvironment env) : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Enter your JWT token like this: Bearer {token}",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });


        foreach (ApiVersionDescription description in apiVersionDescription.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"{Constants.APP_NAME} v{description.ApiVersion} ({env.EnvironmentName})",
                Version = description.ApiVersion.ToString(),
                Description = "API documentation for AspNetCoreBoilerplate",
                Contact = new OpenApiContact
                {
                    Name = "AspNetCoreBoilerplate",
                    Email = "aspnetcoreboilerplate@example.com",
                    Url = new Uri("https://aspnetcoreboilerplate.com")
                }
            });
        }
    }

    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);
}
