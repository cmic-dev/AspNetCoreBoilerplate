namespace AspNetCoreBoilerplate.Web.Core.Models;

public class ErrorResponse
{
    // RFC 9110 Problem Details fields
    public string? Type { get; init; }
    public string? Title { get; init; }
    public int? Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }

    // Validation errors
    public Dictionary<string, string[]>? Errors { get; init; }

    // Tracking fields
    public string? RequestId { get; init; }
    public string? TraceId { get; init; }
}