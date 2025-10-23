using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Core.Extensions;
using AspNetCoreBoilerplate.Web;

using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{ BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Configuration.AddJsonFile("appsettings.json");


builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudBlazorConfiguration();
builder.Services.AddFluxorConfiguration();
builder.Services.AddApiServiceConfiguration(builder.Configuration["API"]!);

// Authentication Services
builder.Services.AddAuthorizationCoreConfiguration();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

await builder.Build().RunAsync();
