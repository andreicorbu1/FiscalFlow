﻿using System.ComponentModel.DataAnnotations;
using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Transactions;

public class AddTransactionRequest
{
    [MaxLength(250)] public string Description { get; set; } = string.Empty;

    [MaxLength(100)] public string Payee { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Value { get; set; }
    public Category Category { get; set; }
    public bool IsRecursive { get; set; } = false;
    public ushort? Recurrence { get; set; } = null;
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? DateModified { get; set; } = null;
    public double? Latitude { get; set; } = null;
    public double? Longitude { get; set; } = null;
    public Guid AccountId { get; set; }
}