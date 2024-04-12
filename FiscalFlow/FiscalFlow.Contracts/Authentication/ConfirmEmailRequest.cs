using System.ComponentModel.DataAnnotations;

namespace FiscalFlow.Contracts.Authentication;

public class ConfirmEmailRequest
{
    [Required] public string? Token { get; set; }

    [Required]
    [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }
}