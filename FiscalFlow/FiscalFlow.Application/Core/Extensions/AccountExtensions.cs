using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Application.Core.Extensions;

public static class AccountExtensions
{
    public static AccountDto ToAccountDto(this Account account)
    {
        var accountDto = new AccountDto()
        {
            Name = account.Name!,
            AccountType = account.AccountType!,
            Balance = account.Balance.Amount,
            Currency = account.MoneyCurrency.ToString()
        };
        return accountDto;
    }
}