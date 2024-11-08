﻿using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Application.Tools.MoneyConverter;
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
    private readonly IImageService _imageService;

    public TransactionService(ITransactionRepository transactionRepository, IAccountService accountService, IRecursiveTransactionRepository recursiveTransactionRepository, IImageService imageService)
    {
        _transactionRepository = transactionRepository;
        _accountService = accountService;
        _recursiveTransactionRepository = recursiveTransactionRepository;
        _imageService = imageService;
    }

    public async Task<Result<Transaction>> AddTransaction(AddTransactionRequest payload, string ownerId)
    {
        var account = await _accountService.GetAccountFromIdAsync(payload.AccountId, ownerId);
        var secondAccount = await _accountService.GetAccountFromNameAndOwner(ownerId, payload.Payee);
        if (!account.IsSuccess)
        {
            switch (account.Status)
            {
                case ResultStatus.NotFound:
                    return Result.NotFound(account.Errors.ElementAt(0));
                case ResultStatus.Forbidden:
                case ResultStatus.Unauthorized:
                    return Result.Unauthorized();
                default:
                    return Result.Error();
            }
        }
        string? imagePublicId = null, imageUrl = null;
        if (!string.IsNullOrEmpty(payload.ImageBase64Encoded))
            imagePublicId = await _imageService.UploadImageAsync(payload.ImageBase64Encoded);
        if (!string.IsNullOrEmpty(imagePublicId))
        {
            imageUrl = await _imageService.GetImageAsync(imagePublicId);
        }
        var accountValue = account.Value;
        var transaction = new Transaction
        {
            Account = accountValue,
            AccountId = payload.AccountId,
            MoneyValue = payload.Value,
            MoneyCurrency = accountValue.MoneyCurrency,
            Description = payload.Description,
            ImagePublicId = imagePublicId,
            ImageUrl = imageUrl,
            Payee = payload.Payee,
            Latitude = payload.Latitude,
            Longitude = payload.Longitude,
            Type = payload.Type,
            Category = payload.Category,
            CreatedOnUtc = payload.CreatedOnUtc,
            AccountValueBefore = accountValue.MoneyBalance
        };

        // TODO: Add option to Convert from one MyCurrency to the account currency
        // Add option get the conversion rate from some 3rd party API
        Money updatedBalance;
        if (transaction.Type == TransactionType.Income)
        {
            if (!secondAccount.IsSuccess)
                updatedBalance = accountValue.Balance + transaction.Value;
            else
            {
                var second = secondAccount.Value;
                decimal value = default;
                if (second.MoneyCurrency != transaction.MoneyCurrency)
                {
                    bool foundConversion = MoneyConversionUtils.Conversions.TryGetValue($"{transaction.MoneyCurrency}-{second.MoneyCurrency}", out value);
                    if (!foundConversion)
                    {
                        updatedBalance = accountValue.Balance - transaction.Value;
                    }
                    else
                    {
                        if (second.MoneyBalance < transaction.MoneyValue * value)
                        {
                            return Result.Error("Could not transfer money from second account because the balance is lower than current transaction!");
                        }
                        updatedBalance = second.Balance - (transaction.Value * value).Amount;
                        var tr = new Transaction
                        {
                            Account = second,
                            AccountId = second.Id,
                            AccountValueAfter = updatedBalance.Amount,
                            AccountValueBefore = second.MoneyBalance,
                            Category = Category.Finance,
                            CreatedOnUtc = transaction.CreatedOnUtc,
                            ModifiedOnUtc = DateTime.UtcNow,
                            Description = $"Transfer to {accountValue.Name}",
                            MoneyValue = transaction.MoneyValue * value,
                            MoneyCurrency = second.MoneyCurrency,
                            Latitude = transaction.Latitude,
                            Longitude = transaction.Longitude,
                            ImagePublicId = transaction.ImagePublicId,
                            ImageUrl = transaction.ImageUrl,
                            Payee = accountValue.Name,
                            Type = TransactionType.Expense,
                        };
                        _transactionRepository.Add(tr);
                        second.MoneyBalance = updatedBalance.Amount;
                        _accountService.UpdateAccount(second);
                    }
                }
                else
                {
                    if (second.MoneyBalance < transaction.MoneyValue)
                    {
                        return Result.Error("Could not transfer money from second account because the balance is lower than current transaction!");
                    }
                    updatedBalance = second.Balance - transaction.Value;
                    var tr = new Transaction
                    {
                        Account = second,
                        AccountId = second.Id,
                        AccountValueAfter = updatedBalance.Amount,
                        AccountValueBefore = second.MoneyBalance,
                        Category = Category.Finance,
                        CreatedOnUtc = DateTime.UtcNow,
                        ModifiedOnUtc = DateTime.UtcNow,
                        Description = $"Transfer to {accountValue.Name}",
                        MoneyValue = transaction.MoneyValue,
                        MoneyCurrency = transaction.MoneyCurrency,
                        Latitude = transaction.Latitude,
                        Longitude = transaction.Longitude,
                        ImagePublicId = transaction.ImagePublicId,
                        ImageUrl = transaction.ImageUrl,
                        Payee = accountValue.Name,
                        Type = TransactionType.Expense,
                    };
                    _transactionRepository.Add(tr);
                    second.MoneyBalance = updatedBalance.Amount;
                    _accountService.UpdateAccount(second);
                }
            }
        }
        else
        {
            if (accountValue.Balance >= transaction.Value)
            {
                updatedBalance = accountValue.Balance - transaction.Value;
                Money updatedBalance2;
                if (secondAccount.IsSuccess)
                {
                    var second = secondAccount.Value;
                    decimal value = 1;
                    if (second.MoneyCurrency != accountValue.MoneyCurrency)
                    {
                        bool foundConversion = MoneyConversionUtils.Conversions.TryGetValue($"{accountValue.MoneyCurrency}-{second.MoneyCurrency}", out value);
                        if (!foundConversion)
                        {
                            updatedBalance2 = accountValue.Balance + transaction.MoneyValue;
                        }
                        else
                        {
                            updatedBalance2 = accountValue.Balance + transaction.MoneyValue * value;
                        }
                    }
                    else
                    {
                        updatedBalance2 = accountValue.Balance + transaction.MoneyValue;
                    }
                    var tr = new Transaction
                    {
                        Account = second,
                        AccountId = second.Id,
                        AccountValueAfter = updatedBalance2.Amount,
                        AccountValueBefore = second.MoneyBalance,
                        Category = Category.Income,
                        CreatedOnUtc = transaction.CreatedOnUtc,
                        ModifiedOnUtc = DateTime.UtcNow,
                        Description = $"Transfer from {accountValue.Name}",
                        MoneyValue = transaction.MoneyValue * value,
                        MoneyCurrency = second.MoneyCurrency,
                        Latitude = transaction.Latitude,
                        Longitude = transaction.Longitude,
                        ImagePublicId = transaction.ImagePublicId,
                        ImageUrl = transaction.ImageUrl,
                        Payee = accountValue.Name,
                        Type = TransactionType.Income,
                    };
                    _transactionRepository.Add(tr);
                    second.MoneyBalance = updatedBalance2.Amount;
                    _accountService.UpdateAccount(second);
                }
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
                Recurrence = (ushort)((payload.Recurrence - 1) ?? 0),
                UserId = ownerId,
                CreatedOnUtc = transaction.CreatedOnUtc
            };
            rt.Transactions.Add(transaction);
            _recursiveTransactionRepository.Add(rt);
            transaction.RecursiveTransactionId = rt.Id;
            _transactionRepository.Update(transaction);
        }
        return Result.Success(transaction);
    }

    public async Task<Result> UpdateTransaction(UpdateTransaction payload, string ownerId)
    {
        var oldTransaction = await _transactionRepository.GetByIdIncludingAccountReccursiveTransactionAsync(payload.TransactionId);
        if (oldTransaction is null)
        {
            return Result.NotFound($"Transaction with id {payload.TransactionId} does not exist!");
        }

        var account = oldTransaction.Account;
        if (account.UserId != ownerId)
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
        if (payload.Recurrence is not null)
        {
            var rt = oldTransaction.RecursiveTransaction;
            if (rt is null)
            {
                rt = new RecursiveTransaction
                {
                    Recurrence = payload.Recurrence.Value,
                    UserId = ownerId
                };
                _recursiveTransactionRepository.Add(rt);
                oldTransaction.RecursiveTransactionId = rt.Id;
            }
            else if (rt is not null && payload.Recurrence.Value != rt.Recurrence)
            {
                rt.Recurrence = payload.Recurrence.Value;
                _recursiveTransactionRepository.Update(rt);
                oldTransaction.RecursiveTransactionId = rt.Id;
            }
        }
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
        var transaction = _transactionRepository.GetByIdIncludingAccountReccursiveTransaction(transactionId);
        if (transaction is null)
            return Result.NotFound($"Transaction with id {transactionId} does not exist!");
        if (transaction.Account.UserId != ownerId)
            return Result.Unauthorized();
        if (!string.IsNullOrEmpty(transaction.ImagePublicId))
            Task.Run(() => _imageService.DeleteImageAsync(transaction.ImagePublicId));
        var rt = transaction.RecursiveTransaction;
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
        if (rt is not null)
        {
            if (rt.Transactions.Count <= 1)
            {
                _recursiveTransactionRepository.Remove(rt);
            }
            else
            {
                rt.Recurrence++;
                _recursiveTransactionRepository.Update(rt);
            }
        }
        _transactionRepository.Remove(transaction);
        return Result.Success();
    }

    public async Task<Result<IList<RecursiveTransactionDto>>> GetSubscriptions(string ownerId, Guid? accountId = null)
    {
        IList<RecursiveTransactionDto> transactions = new List<RecursiveTransactionDto>();
        var rt = await _recursiveTransactionRepository.GetRecursiveTransactions(ownerId, accountId);
        if (rt.Count == 0 || rt is null)
        {
            return Result.Error("You don't have any subscriptions");
        }
        foreach (var tr in rt)
        {
            var transactionsList = tr.Transactions.OrderBy(tr => tr.CreatedOnUtc);

            var trDto = new RecursiveTransactionDto
            {
                Id = tr.Id,
                Account = transactionsList.First().Account.Name,
                Currency = transactionsList.First().MoneyCurrency.ToString(),
                FirstPayment = transactionsList.First().CreatedOnUtc,
                LastPayment = transactionsList.Last().CreatedOnUtc,
                Payee = transactionsList.First().Payee,
                RemainingPayments = tr.Recurrence,
                Value = transactionsList.First().Type == TransactionType.Income ? transactionsList.First().MoneyValue : -transactionsList.First().MoneyValue
            };
            transactions.Add(trDto);
        }
        return Result.Success(transactions);
    }

    public Result DeleteRecursiveTransaction(string ownerId, Guid recursiveTransactionId)
    {
        var rt = _recursiveTransactionRepository.GetByIdIncludingTransaction(recursiveTransactionId);
        if (rt.UserId != ownerId)
        {
            return Result.Forbidden();
        }
        _recursiveTransactionRepository.Remove(rt);
        return Result.Success();
    }
}