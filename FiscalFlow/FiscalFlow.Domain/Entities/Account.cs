using FiscalFlow.Domain.Core.Primitives;
using FiscalFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NodaMoney;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FiscalFlow.Domain.Entities;

[Index(nameof(Name), nameof(UserId), IsUnique = true)]
public class Account : BaseEntity
{
    [MaxLength(50)] public string? Name { get; set; }
    [NotMapped] public Money Balance => new(MoneyBalance, MoneyCurrency.ToString());
    [JsonIgnore][Column("Balance")] public decimal MoneyBalance { get; set; }
    [JsonIgnore][Column("Currency")] public MyCurrency MoneyCurrency { get; set; }
    public AccountType AccountType { get; set; }
    [MaxLength(36)] public string UserId { get; set; } = Guid.Empty.ToString();
    public virtual AppUser User { get; set; } = null!;
    public virtual IList<Transaction>? Transactions { get; set; } = null!;
}