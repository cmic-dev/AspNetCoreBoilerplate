using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;
using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Services;

internal class RoleService(IAppDbContext dbContext) : IRoleService
{
    public async Task<List<RoleDto>> GetRolesAsync(CancellationToken ctn = default)
    {
        var roles = await dbContext.Set<Role>()
            .Select(x => new RoleDto
            {
                Id = x.Id,
                Name = x.Name,
                DisplayName = x.DisplayName,
            })
            .ToListAsync();
        return roles;
    }
}
