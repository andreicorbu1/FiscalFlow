﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FiscalFlow.Domain.Core.Abstractions;
using FiscalFlow.Domain.Core.Primitives;
using FiscalFlow.Domain.Enums;
using NodaMoney;
using MyCurrency = FiscalFlow.Domain.Enums.MyCurrency;

namespace FiscalFlow.Domain.Entities;

public class Transaction : BaseEntity, IAuditableEntity
{
    [NotMapped] public Money Value => new Money(MoneyValue, MoneyCurrency.ToString());
    [JsonIgnore] [Column("Value")] public decimal MoneyValue { get; set; }
    [JsonIgnore] [Column("Currency")] public MyCurrency MoneyCurrency { get; set; }
    [Column("AccountValueBeforeTransaction")] public decimal AccountValueBefore { get; set; }
    [Column("AccountValueAfterTransaction")] public decimal AccountValueAfter { get; set; }
    [MaxLength(250)] public string Description { get; set; } = string.Empty;

    [MaxLength(100)] public string Payee { get; set; } = string.Empty;
    public IList<string> Labels { get; set; } = new List<string>();
    public TransactionType Type { get; set; }
    public Category Category { get; set; }
    public DateTime CreatedOnUtc { get; init; } = DateTime.UtcNow;

    public DateTime? ModifiedOnUtc { get; set; }

    public Guid AccountId { get; set; }
    public virtual Account? Account { get; set; } = null!;
}