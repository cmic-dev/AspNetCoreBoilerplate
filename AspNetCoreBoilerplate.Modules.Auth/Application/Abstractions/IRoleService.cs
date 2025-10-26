using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;

public interface IRoleService
{
    Task<List<RoleDto>> GetRolesAsync(CancellationToken ctn = default);
}
