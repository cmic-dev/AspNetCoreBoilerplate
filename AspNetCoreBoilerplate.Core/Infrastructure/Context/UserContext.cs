using AspNetCoreBoilerplate.Core.Models;
using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UAParser;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Context;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Lazy<ClinetUserAgent?> _parsedUserAgent;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _parsedUserAgent = new Lazy<ClinetUserAgent?>(ParseUserAgent);
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? UserName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Name)?.Value;
        }
    }

    public string? IpAddress
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // Check for forwarded IP addresses (when behind a proxy/load balancer)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                return forwardedFor.Split(',').FirstOrDefault()?.Trim();
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fallback to direct connection IP
            return httpContext.Connection?.RemoteIpAddress?.ToString();
        }
    }

    public string? UserAgent =>
        _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();

    public string? Device => _parsedUserAgent.Value?.Device;

    public string? Platform => _parsedUserAgent.Value?.Platform;

    public string? Browser => _parsedUserAgent.Value?.Browser;

    private ClinetUserAgent? ParseUserAgent()
    {
        var userAgent = UserAgent;

        if (string.IsNullOrWhiteSpace(userAgent))
            return null;

        try
        {
            var parser = Parser.GetDefault();
            var clientInfo = parser.Parse(userAgent);

            var device = clientInfo.Device.Family != "Other"
                ? clientInfo.Device.Family
                : null;

            var platform = !string.IsNullOrEmpty(clientInfo.OS.Family)
                ? $"{clientInfo.OS.Family} {clientInfo.OS.Major}".Trim()
                : null;

            var browser = !string.IsNullOrEmpty(clientInfo.UA.Family)
                ? $"{clientInfo.UA.Family} {clientInfo.UA.Major}".Trim()
                : null;

            return new ClinetUserAgent(device, platform, browser);
        }
        catch
        {
            return null;
        }
    }
}
