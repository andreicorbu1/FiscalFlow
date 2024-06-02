using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Contracts.Transactions;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Enums;
using FiscalFlow.Domain.Repositories;
using NodaMoney;

namespace FiscalFlow.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IRecursiveTransactionRepository _recursiveTransactionRepository;
    private readonly IAccountService _accountService;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository, IAccountService accountService, IRecursiveTransactionRepository recursiveTransactionRepository)
    {
        _transactionRepository = transactionRepository;
        _accountService = accountService;
        _recursiveTransactionRepository = recursiveTransactionRepository;
    }

    public async Task<Result<Transaction>> AddTransaction(AddTransactionRequest payload, string ownerId)
    {
        var account = await _accountService.GetAccountFromIdAsync(payload.AccountId, ownerId);
        if (!account.IsSuccess)
            switch (account.Status)
            {
                case ResultStatus.NotFound:
                    return Result.NotFound(account.Errors[0]);
                case ResultStatus.Forbidden:
                case ResultStatus.Unauthorized:
                    return Result.Unauthorized();
                default:
                    return Result.Error();
            }

        var accountValue = account.Value;
        var transaction = new Transaction
        {
            Account = accountValue,
            AccountId = payload.AccountId,
            MoneyValue = payload.Value,
            MoneyCurrency = accountValue.MoneyCurrency,
            Description = payload.Description,
            Payee = payload.Payee,
            Latitude = payload.Latitude,
            Longitude = payload.Longitude,
            Type = payload.Type,
            Category = payload.Category,
            CreatedOnUtc = payload.CreatedOnUtc,
            AccountValueBefore = accountValue.MoneyBalance
        };

        // Add option to Convert from one MyCurrency to the account currency
        // Add option get the conversion rate from some 3rd party API
        Money updatedBalance;
        if (transaction.Type == TransactionType.Income)
        {
            updatedBalance = accountValue.Balance + transaction.Value;
        }
        else
        {
            if (accountValue.Balance - transaction.Value >= 0)
            {
                updatedBalance = accountValue.Balance - transaction.Value;
            }
            else
            {
                return Result.Error("This transaction value is bigger than your account's balance.");
            }
        }
        accountValue.MoneyBalance = updatedBalance.Amount;
        transaction.AccountValueAfter = accountValue.MoneyBalance;
        _accountService.UpdateAccount(account);
        var id = _transactionRepository.Add(transaction);
        if (payload.IsRecursive)
        {
            var rt = new RecursiveTransaction
            {
                LastTransaction = transaction,
                Recurrence = payload.Recurrence ?? 0,
                TransactionId = id,
                OwnerId = ownerId
            };
            _recursiveTransactionRepository.Add(rt);
        }
        return Result.Success(transaction);
    }

    public async Task<Result> UpdateTransaction(UpdateTransaction payload, string ownerId)
    {
        var oldTransaction = await _transactionRepository.GetByIdIncludingAccountAsync(payload.TransactionId);
        if (oldTransaction is null)
        {
            return Result.NotFound($"Transaction with id {payload.TransactionId} does not exist!");
        }

        var account = oldTransaction.Account;
        if (account.OwnerId != ownerId)
        {
            return Result.Unauthorized();
        }

        Money updatedBalance;
        if (oldTransaction.Type == TransactionType.Income)
        {
            updatedBalance = account.Balance - oldTransaction.Value;
        }
        else
        {
            updatedBalance = account.Balance + oldTransaction.Value;
        }

        account.MoneyBalance = updatedBalance.Amount;

        oldTransaction.AccountValueBefore = updatedBalance.Amount;
        oldTransaction.AccountId = account.Id;
        oldTransaction.Category = payload.Category;
        oldTransaction.CreatedOnUtc = payload.CreatedOnUtc;
        oldTransaction.Description = payload.Description;
        oldTransaction.Latitude = payload.Latitude;
        oldTransaction.Longitude = payload.Longitude;
        oldTransaction.ModifiedOnUtc = DateTime.UtcNow;
        oldTransaction.Payee = payload.Payee;
        oldTransaction.Type = payload.Type;
        oldTransaction.MoneyValue = payload.Value;
        if (payload.Type == TransactionType.Income)
        {
            updatedBalance = account.MoneyBalance + payload.Value;
        }
        else
        {
            updatedBalance = account.MoneyBalance - payload.Value;
        }

        account.MoneyBalance = updatedBalance.Amount;
        oldTransaction.AccountValueAfter = account.MoneyBalance;
        _accountService.UpdateAccount(account);
        _transactionRepository.Update(oldTransaction);
        return Result.Success();
    }

    public async Task<Result<IList<Transaction>>> GetTransactionsFromAccountPeriodOfTime(string ownerId, Guid accountId,
        PeriodOfTimeRequest period)
    {
        var accountExists = _accountService.CheckAccountExistsAndHasAccess(accountId, ownerId);
        if (!accountExists.IsSuccess)
            return accountExists;
        var startDate = new DateTime(period.StartDate.Year, period.StartDate.Month, period.StartDate.Day);
        var endDate = new DateTime(period.EndDate.Year, period.EndDate.Month, period.EndDate.Day, 23, 59, 59);
        var transactions =
            await _transactionRepository.GetTransactionsFromAccountInPeriodOfTime(accountId, startDate, endDate);
        return Result.Success(transactions);
    }

    public Result DeleteTransaction(string ownerId, Guid transactionId)
    {
        var transaction = _transactionRepository.GetByIdIncludingAccount(transactionId);
        if (transaction is null)
            return Result.NotFound($"Transaction with id {transactionId} does not exist!");
        if (transaction.Account.OwnerId != ownerId)
            return Result.Unauthorized();
        var account = transaction.Account;
        Money updatedBalance;
        if (transaction.Type == TransactionType.Income)
        {
            updatedBalance = account.Balance - transaction.Value;
        }
        else
        {
            updatedBalance = account.Balance + transaction.Value;
        }

        account.MoneyBalance = updatedBalance.Amount;
        _accountService.UpdateAccount(account);
        _transactionRepository.Remove(transaction);
        return Result.Success();
    }
}