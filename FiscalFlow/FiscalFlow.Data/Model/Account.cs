using System.Collections;
using FiscalFlow.Model.Enum;
using NodaMoney;

namespace FiscalFlow.Model;

public class Account : BaseEntity
{
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public string Name { get; set; }
    public Money Balance { get; set; }
    public AccountType AccountType { get; set; }
    public IEnumerable Transactions { get; set; }
}