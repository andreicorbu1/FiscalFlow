using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Transactions;

public class UpdateTransaction
{
    public Guid TransactionId { get; set; }
    public string Description { get; set; }
    public string Payee { get; set; }
    public TransactionType Type { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal Value { get; set; }
    public ushort? Recurrence { get; set; }
    public Category Category { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}