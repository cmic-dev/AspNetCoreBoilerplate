namespace AspNetCoreBoilerplate.Core.Models;

public class ClinetUserAgent
{
    public ClinetUserAgent(string? device, string? platform, string? browser)
    {
        Device = device;
        Platform = platform;
        Browser = browser;
    }

    public string? Device { get; set; }
    public string? Platform { get; set; }
    public string? Browser { get; set; }
}
