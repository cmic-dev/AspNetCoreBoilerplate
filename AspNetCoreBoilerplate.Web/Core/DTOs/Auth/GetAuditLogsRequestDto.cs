namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class GetAuditLogsRequestDto
{
    public Guid? UserId { get; set; }
    public string? Action { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
