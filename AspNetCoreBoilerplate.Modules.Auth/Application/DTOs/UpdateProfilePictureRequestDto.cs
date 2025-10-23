using Microsoft.AspNetCore.Http;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class UpdateProfilePictureRequestDto
{
    public IFormFile? ProfilePicture { get; set; }
}
