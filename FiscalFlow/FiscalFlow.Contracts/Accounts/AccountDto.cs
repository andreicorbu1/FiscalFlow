using System.ComponentModel.DataAnnotations;
using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Accounts;

public class AccountDto
{
    public Guid Id { get; set; }
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public AccountType AccountType { get; set; }
    public IList<TransactionDto> Transactions { get; set; }
}