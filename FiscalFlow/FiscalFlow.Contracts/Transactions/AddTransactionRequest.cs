using FiscalFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiscalFlow.Contracts.Transactions;

public class AddTransactionRequest
{
    [MaxLength(250)] public string Description { get; set; } = string.Empty;

    [MaxLength(100)] public string Payee { get; set; } = string.Empty;
    public IList<string> Labels { get; set; } = new List<string>();
    public TransactionType Type { get; set; }
    public decimal Value { get; set; }
    public Category Category { get; set; }
    public Guid AccountId { get; set; }
}
