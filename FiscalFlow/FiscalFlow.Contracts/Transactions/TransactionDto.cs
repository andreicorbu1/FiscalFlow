using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Accounts;

public class TransactionDto
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Currency { get; set; }
    public decimal AccountValueBefore { get; set; }
    public decimal AccountValueAfter { get; set; }
    public string Description { get; set; }
    public string Payee { get; set; }
    public TransactionType Type { get; set; }
    public Category Category { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string Account { get; set; }
}