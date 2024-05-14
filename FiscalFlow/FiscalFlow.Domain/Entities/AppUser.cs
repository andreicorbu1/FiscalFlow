using System.ComponentModel.DataAnnotations;
using FiscalFlow.Domain.Core.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace FiscalFlow.Domain.Entities;

public class AppUser : IdentityUser, IAuditableEntity
{
    [Required] public string? FirstName { get; set; } = string.Empty;

    [Required] public string? LastName { get; set; } = string.Empty;

    public string? Provider { get; set; }
    public virtual IList<Account> Accounts { get; set; } = new List<Account>();
    public DateTime CreatedOnUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? RefreshToken { get; set; } = string.Empty;
}