using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Application.Core.Extensions;

public static class TransactionExtensions
{
    public static TransactionDto ToTransactionDto(this Transaction transaction)
    {
        var transactionDto = new TransactionDto()
        {
            AccountValueAfter = transaction.AccountValueAfter,
            AccountValueBefore = transaction.AccountValueBefore,
            Category = transaction.Category,
            Currency = transaction.Value.Currency.ToString()!,
            Description = transaction.Description,
            Payee = transaction.Payee,
            Type = transaction.Type,
            Value = transaction.Value.Amount,
            CreatedOnUtc = transaction.CreatedOnUtc,
            Id = transaction.Id
        };
        return transactionDto;
    }
}