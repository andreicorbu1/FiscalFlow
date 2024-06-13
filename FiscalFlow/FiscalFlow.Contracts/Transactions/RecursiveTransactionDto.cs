using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Transactions;

public class RecursiveTransactionDto
{
    public Guid Id { get; set; }
    public string Account { get; set; }
    public string Payee { get; set; }
    public decimal Value { get; set; }
    public string Currency { get; set; }
    public DateTime FirstPayment { get; set; }
    public DateTime LastPayment { get; set; }
    public uint RemainingPayments { get; set; }
}
