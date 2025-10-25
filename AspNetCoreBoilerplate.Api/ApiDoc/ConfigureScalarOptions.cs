using AspNetCoreBoilerplate.Shared;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace AspNetCoreBoilerplate.Api.ApiDoc;

public class ConfigureScalarOptions(IApiVersionDescriptionProvider apiVersionDescription) : IConfigureOptions<ScalarOptions>
{
    public void Configure(ScalarOptions options)
    {
        options.WithTitle($"{Constants.APP_NAME} | API Documents");

        foreach (var description in apiVersionDescription.ApiVersionDescriptions)
        {
            var isDefault = description.GroupName == "v1";
            options.AddDocument(
                documentName: description.GroupName,
                title: $"{Constants.APP_NAME} {description.GroupName.ToUpperInvariant()}",
                routePattern: $"/swagger/{description.GroupName}/swagger.json",
                isDefault: isDefault
            );
        }
        options.EnablePersistentAuthentication();
        options.EnableDarkMode();

        options.AddHttpAuthentication("Bearer", httpScheme => { });
        options.AddPreferredSecuritySchemes(["Bearer"]);
    }
}
