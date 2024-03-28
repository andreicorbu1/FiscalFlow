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

    public async Task<Result> AddTransaction(AddTransactionRequest payload)
    {
        var account = await _accountService.GetAccountFromId(payload.AccountId);
        if(!account.IsSuccess)
        {
            return Result.NotFound($"Account with id {payload.AccountId} does not exist");
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
        // Add option to Convert from one Currency to the account currency
        // Add option get the conversion rate from some 3rd party API

        NodaMoney.Money updatedBalance = accountValue.Balance - transaction.Value;
        accountValue.MoneyBalance = updatedBalance.Amount;
        transaction.AccountValueAfter = accountValue.MoneyBalance;
        _accountService.UpdateAccount(account);
        _transactionRepository.Add(transaction);
        return Result.Success();
    }
}
