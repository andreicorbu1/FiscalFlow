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
            Type = account.AccountType!.ToString(),
            Balance = account.Balance.Amount,
            Currency = account.MoneyCurrency.ToString(),
            CreatedOnUtc = account.CreatedOnUtc,
            Transactions = new List<TransactionDto>(),
            Id = account.Id
        };
        if (account.Transactions is { Count: > 0 })
        {
            foreach (var transaction in account.Transactions)
            {
                var transactionDto = transaction.ToTransactionDto();
                transactionDto.Account = account.Name!;
                accountDto.Transactions.Add(transactionDto);
            }
        }
        return accountDto;
    }
}