using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FiscalFlow.Model;

public class AppUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public string Provider { get; set; }
}