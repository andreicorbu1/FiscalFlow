using FiscalFlow.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace FiscalFlow.Contracts.Accounts;

public class CreateAccountRequest
{
    [MaxLength(50)]
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public Currency Currency { get; set; }
    public AccountType AccountType { get; set; }
    public string OwnerId { get; set; }
}
