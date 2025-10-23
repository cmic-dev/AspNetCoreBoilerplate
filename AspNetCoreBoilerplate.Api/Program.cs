using AspNetCoreBoilerplate.Api.OpenApi;
using AspNetCoreBoilerplate.Api.ProblemDetails;
using AspNetCoreBoilerplate.Api.Cache;
using AspNetCoreBoilerplate.Api.RateLimiter;
using AspNetCoreBoilerplate.Api.ApiVersioning;
using AspNetCoreBoilerplate.Api.Cors;
using AspNetCoreBoilerplate.Api.Authorization;
using AspNetCoreBoilerplate.Core;
using AspNetCoreBoilerplate.Core.Extensions;
using AspNetCoreBoilerplate.Modules.Auth;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services.
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

// ProblemDetails
builder.Services.ConfigureOptions<ConfigureProblemDetailsOptions>();
builder.Services.AddProblemDetails();

// Swagger
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();
builder.Services.ConfigureOptions<ConfigureSwaggerUIOptions>();
builder.Services.AddSwaggerGen();

// Scalar
builder.Services.ConfigureOptions<ConfigureScalarOptions>();

// Cache
builder.Services.ConfigureOptions<ConfigureHybridCacheOptions>();
builder.Services.AddHybridCache();

// RateLimiter
builder.Services.ConfigureOptions<ConfigureRateLimiterOptions>();
builder.Services.AddRateLimiter();

// ApiVersioning
builder.Services.ConfigureOptions<ConfigureApiVersioningOptions>();
builder.Services.ConfigureOptions<ConfigureApiExplorerOptions>();
builder.Services.AddApiVersioning();
builder.Services.AddVersionedApiExplorer();

// Cors
builder.Services.ConfigureOptions<ConfigureCorsOptions>();
builder.Services.AddCors();

// Auth
builder.Services.ConfigureOptions<ConfigureAuthorizationOptions>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Modules
builder.ConfigureModules(orchestrator =>
{
    orchestrator.AddModule(new CoreModule());
    orchestrator.AddModule(new AuthModule());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.InitializeModulesAndRunAsync();
