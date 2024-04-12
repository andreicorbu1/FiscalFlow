using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Contracts.Transactions;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;

namespace FiscalFlow.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IAccountService _accountService;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(ITransactionRepository transactionRepository, IAccountService accountService)
    {
        _transactionRepository = transactionRepository;
        _accountService = accountService;
    }

    public async Task<Result> AddTransaction(AddTransactionRequest payload, string ownerId)
    {
        var account = await _accountService.GetAccountFromIdAsync(payload.AccountId, ownerId);
        if(!account.IsSuccess)
        {
            switch (account.Status)
            {
                case ResultStatus.NotFound:
                    return Result.NotFound(account.Errors.First());
                case ResultStatus.Forbidden:
                case ResultStatus.Unauthorized:
                        return Result.Unauthorized();
                default:
                    return Result.Error();
            }
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
            Labels = payload.Labels,
            Type = payload.Type,
            Category = payload.Category,
            AccountValueBefore = accountValue.MoneyBalance
        };
        // Add option to Convert from one MyCurrency to the account currency
        // Add option get the conversion rate from some 3rd party API

        NodaMoney.Money updatedBalance = accountValue.Balance - transaction.Value;
        accountValue.MoneyBalance = updatedBalance.Amount;
        transaction.AccountValueAfter = accountValue.MoneyBalance;
        _accountService.UpdateAccount(account);
        _transactionRepository.Add(transaction);
        return Result.Success();
    }

    public Result DeleteTransaction(string ownerId, Guid transactionId)
    {
        var transaction = _transactionRepository.GetByIdIncludingAccount(transactionId);
        if (transaction is null)
            return Result.NotFound($"Transaction with id {transactionId} does not exist!");
        if (transaction.Account?.OwnerId != ownerId)
            return Result.Unauthorized();
        _transactionRepository.Remove(transaction);
        return Result.Success();
    }
}
