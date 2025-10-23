namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class GetLoginHistoriesRequestDto
{
    public Guid? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
