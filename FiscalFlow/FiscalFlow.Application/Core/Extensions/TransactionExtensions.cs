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
            Currency = transaction.Value.Currency.Code,
            Description = transaction.Description,
            Latitude = transaction.Latitude,
            Longitude = transaction.Longitude,
            ImageUrl = transaction.ImageUrl,
            Payee = transaction.Payee,
            ReccurencePeriod = transaction.RecursiveTransaction?.Recurrence,
            Type = transaction.Type,
            Value = transaction.Value.Amount,
            CreatedOnUtc = transaction.CreatedOnUtc,
            Id = transaction.Id
        };
        if (transaction.Account is not null)
        {
            transactionDto.Account = transaction.Account.Name!;
        }
        return transactionDto;
    }
}