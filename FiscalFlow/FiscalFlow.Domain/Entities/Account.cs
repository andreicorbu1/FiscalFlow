using FiscalFlow.Domain.Core.Abstractions;
using FiscalFlow.Domain.Core.Primitives;
using FiscalFlow.Domain.Enums;
using NodaMoney;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FiscalFlow.Domain.Entities;

public class Account : BaseEntity, IAuditableEntity
{
    [MaxLength(50)] public string? Name { get; set; }
    [NotMapped] public Money Balance => new Money(MoneyBalance, MoneyCurrency.ToString());
    [JsonIgnore][Column("Balance")] public decimal MoneyBalance { get; set; }
    [JsonIgnore] [Column("Currency")] public MyCurrency MoneyCurrency { get; set; }
    public AccountType AccountType { get; set; }
    public DateTime CreatedOnUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; set; }
    [MaxLength(36)] public string OwnerId { get; set; } = Guid.Empty.ToString();
    public virtual AppUser Owner { get; set; } = null!;
    public virtual IList<Transaction>? Transactions { get; set; }
}