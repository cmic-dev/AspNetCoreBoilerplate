using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class UpdateProfilePictureDto
{
    [Required(ErrorMessage = "Picture URL is required")]
    [StringLength(2048, MinimumLength = 10, ErrorMessage = "Picture URL must be between 10 and 2048 characters")]
    public string PictureUrl { get; set; } = string.Empty;
}
