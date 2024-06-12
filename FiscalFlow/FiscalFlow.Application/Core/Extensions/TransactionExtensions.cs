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
            ReccurencePeriod = (ushort?)CalculateOriginalRecurrence(transaction),
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

    private static int? CalculateOriginalRecurrence(Transaction transaction)
    {
        if (transaction.RecursiveTransaction == null)
        {
            return null;
        }

        var recurrencePeriod = transaction.RecursiveTransaction.Recurrence;
        var creationDate = transaction.CreatedOnUtc;
        var now = DateTime.UtcNow;

        // Calculate the number of months between the transaction creation date and now
        int monthsElapsed = ((now.Year - creationDate.Year) * 12) + now.Month - creationDate.Month;

        // Calculate remaining recurrence period
        int originalRecurrence = recurrencePeriod + monthsElapsed;

        return originalRecurrence >= 0 ? originalRecurrence : 0;
    }
}