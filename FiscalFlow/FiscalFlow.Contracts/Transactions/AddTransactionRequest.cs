using System.ComponentModel.DataAnnotations;
using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Transactions;

public class AddTransactionRequest
{
    [MaxLength(250)] public string Description { get; set; } = string.Empty;

    [MaxLength(100)] public string Payee { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public decimal Value { get; set; }
    public Category Category { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? DateModified { get; set; } = null;
    public Guid AccountId { get; set; }
}