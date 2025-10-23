using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Pages;

public partial class NotFoundPage
{
    [Inject]
    private NavigationManager NavigationManager { get; init; } = default!;

    private void GoBack()
    {
        NavigationManager.NavigateTo("/");
    }
}
