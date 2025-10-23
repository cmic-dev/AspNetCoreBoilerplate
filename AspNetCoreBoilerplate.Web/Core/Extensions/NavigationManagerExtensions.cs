using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class NavigationManagerExtensions
{
    public static string? GetParam(this NavigationManager NavigationManager, string paramName)
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = uri.Query;
        var queryParams = System.Web.HttpUtility.ParseQueryString(query);

        return queryParams[paramName];
    }
}
