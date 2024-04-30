using System.ComponentModel.DataAnnotations;
using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Accounts;

public class AccountDto
{
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; }
    public AccountType AccountType { get; set; }
}