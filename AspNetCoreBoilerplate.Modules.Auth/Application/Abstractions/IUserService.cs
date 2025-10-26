using AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;

public interface IUserService
{
    Task<IEnumerable<UserDetailsDto>> GetAllActiveUsersAsync(CancellationToken ctn = default);
    Task<Guid> CreateUserAsync(CreateUserRequestDto dto, CancellationToken ctn = default);
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ctn = default);
    Task<bool> ActivateUserAsync(Guid userId, CancellationToken ctn = default);
}
