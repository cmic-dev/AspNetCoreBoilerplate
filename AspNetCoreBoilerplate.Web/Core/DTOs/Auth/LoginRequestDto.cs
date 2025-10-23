using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Validation_Required")]
    [Display(Name = "FormField_UserName")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required")]
    [MinLength(3, ErrorMessage = "Validation_MinLength")]
    [MaxLength(100, ErrorMessage = "Validation_MaxLength")]
    [Display(Name = "FormField_Password")]
    public string Password { get; set; } = string.Empty;
}
