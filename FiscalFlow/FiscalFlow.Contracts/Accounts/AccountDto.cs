using System.ComponentModel.DataAnnotations;

namespace FiscalFlow.Contracts.Accounts;

public class AccountDto
{
    public Guid Id { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public string Type { get; set; }
    public IList<TransactionDto> Transactions { get; set; }
}