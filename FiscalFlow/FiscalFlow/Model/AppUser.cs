using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FiscalFlow.Model;

public class AppUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required] 
    public string LastName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}