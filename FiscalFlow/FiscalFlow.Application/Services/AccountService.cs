using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Authentication;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Application.Tools.Csv;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserService _userService;

    public AccountService(IAccountRepository accountRepository, IUserService userService)
    {
        _accountRepository = accountRepository;
        _userService = userService;
    }

    public Result CreateAccount(CreateAccountRequest payload)
    {
        var doesAccountExist = _userService.CheckUserExists(payload.OwnerId);
        if(doesAccountExist)
        {
            var account = new Account
            {
                Name = payload.Name,
                AccountType = payload.AccountType,
                MoneyBalance = payload.Balance,
                MoneyCurrency = payload.Currency,
                OwnerId = payload.OwnerId,
            };
            _accountRepository.Add(account);
            return Result.Success();
        }
        else
        {
            return Result.NotFound($"Account with id {payload.OwnerId} does not exist!");
        }
    }

    public Result DeleteAccount(Guid accountId)
    {
        var account = _accountRepository.GetById(accountId);
        if(account is null)
        {
            return Result.NotFound($"Account with id {accountId} does not exist");
        }
        _accountRepository.Remove(account);
        return Result.Success();
    }

    public async Task ExportTransactionsAsCsvAsync(Guid accountId)
    {
        var transactions = await _accountRepository.GetTransactionsAsync(accountId);
        CsvExporter.ExportList(transactions, $"{accountId}");
    }

    public async Task<Result<Account>> GetAccountFromIdAsync(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        return account is null ? Result.NotFound($"Account with id {accountId} does not exist") : Result.Success(account);
    }

    public Result UpdateAccount(Account account)
    {
        _accountRepository.Update(account);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<Account>>> GetAccountsOfOwnerAsync(string ownerId)
    {
        var accounts = await _accountRepository.GetUserAccountsAsync(ownerId);

        return Result.Success(accounts);
    }

    public Result InsertBulkAccounts()
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UpdateAccount(string ownerId, UpdateAccountRequest account)
    {
        var existingAccount = await _accountRepository.GetAllAsQuery()
            .Include(acc => acc.Transactions)
            .FirstOrDefaultAsync(acc => acc.Id.Equals(account.AccountId) );
 
        if (existingAccount is null)
        {
            return Result.NotFound($"Account with id {account.AccountId} does not exist!");
        }

        if (existingAccount.OwnerId != ownerId)
        {
            return Result.Unauthorized();
        }
        
        existingAccount.Name = account.Name;
        if (existingAccount.MoneyCurrency != account.MoneyCurrency && account.MoneyBalance is null)
        {
            /* existingAccount.MoneyBalance = existingAccount.MoneyBalance / Utils.GetConversionRate(existingAccount.MoneyCurrency ,account.MoneyCurrency);*/
        }
        else if(account.MoneyBalance.HasValue)
        {
            existingAccount.MoneyBalance = account.MoneyBalance.Value;
        }
        existingAccount.MoneyCurrency = account.MoneyCurrency;
        existingAccount.AccountType = account.AccountType;

        if (existingAccount.Transactions is not null && existingAccount.Transactions.Count > 0)
        {
            foreach (var transaction in existingAccount.Transactions)
            {
                transaction.MoneyCurrency = existingAccount.MoneyCurrency;
                /* transaction.MoneyValue /= Utils.GetConversionRate(existingAccount.MoneyCurrency ,account.MoneyCurrency);*/
            }
        }
        
        _accountRepository.Update(existingAccount);
        return Result.Success();
    }
}
