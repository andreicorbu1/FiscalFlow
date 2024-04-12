using System.ComponentModel.DataAnnotations;

namespace FiscalFlow.Contracts.Authentication;

public class RegisterRequest
{
    [Required]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "First name must be at least {2}, and maximum {1} characters")]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Last name must be at least {2}, and maximum {1} characters")]
    public string? LastName { get; set; }

    [Required]
    [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be at least {2}, and maximum {1} characters")]
    public string? Password { get; set; }
}