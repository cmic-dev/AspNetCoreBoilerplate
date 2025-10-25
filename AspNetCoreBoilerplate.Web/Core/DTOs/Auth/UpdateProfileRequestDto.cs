using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class UpdateProfileRequestDto
{
    [Required(ErrorMessage = "Validation_Required")]
    [Display(Name = "FormField_Display_Name")]
    public string FullName { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
}
