using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class UpdateEmailRequestDto
{
    [Required(ErrorMessage = "Validation_Required")]
    [Display(Name = "FormField_New_Email")]
    [EmailAddress(ErrorMessage = "Validation_Email")]
    public string NewEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required")]
    [Display(Name = "FormField_Password")]
    public string Password { get; set; } = string.Empty;
}

