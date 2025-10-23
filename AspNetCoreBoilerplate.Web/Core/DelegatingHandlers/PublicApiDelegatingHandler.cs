using AspNetCoreBoilerplate.Web.Core.Authentication;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace AspNetCoreBoilerplate.Web.Core.DelegatingHandlers;

public class PublicApiDelegatingHandler : DelegatingHandler
{
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<PublicApiDelegatingHandler> _logger;

    public PublicApiDelegatingHandler(NavigationManager navigationManager, ILogger<PublicApiDelegatingHandler> logger)
    {
        _navigationManager = navigationManager;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-Version", "1.0.0");
        var authResponse = await base.SendAsync(request, cancellationToken);

        if (authResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Authentication endpoint returned 401. Redirecting to login.");
            _navigationManager.NavigateTo(AuthConstants.LoginPath, true);

            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = request,
                ReasonPhrase = "Unauthorized"
            };
        }

        return authResponse;
    }
}
