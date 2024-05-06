using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Authentication;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Application.Core.Extensions;
using FiscalFlow.Application.Tools.Csv;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Enums;
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
        if (doesAccountExist)
        {
            var sameName = _accountRepository.CheckAccountWithSameName(payload.OwnerId, payload.Name);
            if (sameName)
            {
                return Result.Conflict($"You already have an account with the name {payload.Name}");
            }
            
            var account = new Account
            {
                Name = payload.Name,
                AccountType = payload.AccountType,
                MoneyBalance = payload.Balance,
                MoneyCurrency = Enum.Parse<MyCurrency>(payload.Currency),
                OwnerId = payload.OwnerId,
                CreatedOnUtc = DateTime.UtcNow
            };
            _accountRepository.Add(account);
            return Result.Success();
        }

        return Result.NotFound($"Account with id {payload.OwnerId} does not exist!");
    }

    public async Task<Result<Dictionary<Category, decimal>>> GetCategoryReportsFromAllAccounts(string ownerId)
    {
        var accounts = await _accountRepository.GetUserAccountsAsync(ownerId);
        var dict = new Dictionary<Category, decimal>();
        foreach (var account in accounts)
        {
            foreach (var transaction in account.Transactions!)
            {
                if (dict.TryGetValue(transaction.Category, out var oldValue))
                {
                    dict[transaction.Category] = oldValue + transaction.MoneyValue;
                }
                else
                {
                    dict.Add(transaction.Category, transaction.MoneyValue);
                }
            }
        }
        Console.WriteLine(dict);
        return Result.Success(dict);
    }

    public async Task<Result<List<TransactionDto>>> GetLastTransactions(string ownerId, int numberOfTransactions)
    {
        var transactions = await _accountRepository.GetLastTransactionsAsync(ownerId, numberOfTransactions);
        var transactionDtos = new List<TransactionDto>();
        foreach (var transaction in transactions)
        {
            transactionDtos.Add(transaction.ToTransactionDto());
        }
        return Result.Success(transactionDtos);
    }

    public Result DeleteAccount(Guid accountId, string ownerId)
    {
        var account = _accountRepository.GetById(accountId);
        if (account is null) return Result.NotFound($"Account with id {accountId} does not exist");

        if (account.OwnerId != ownerId) return Result.Forbidden();

        _accountRepository.Remove(account);
        return Result.Success();
    }


    public Result CheckAccountExistsAndHasAccess(Guid accountId, string ownerId)
    {
        if (!_accountRepository.CheckAccountExists(accountId))
            return Result.NotFound($"Account with id {accountId} does not exist!");
        if (!_accountRepository.CheckIfIsAccountOwner(accountId, ownerId))
            return Result.Forbidden();
        return Result.Success();
    }

    public async Task<Result> ExportTransactionsAsCsvAsync(Guid accountId, string ownerId)
    {
        if (_accountRepository.CheckAccountExists(accountId))
        {
            if (_accountRepository.CheckIfIsAccountOwner(accountId, ownerId))
            {
                var transactions = await _accountRepository.GetTransactionsAsync(accountId);
                CsvExporter.ExportList(transactions, $"{accountId}");
                return Result.Success();
            }

            return Result.Forbidden();
        }

        return Result.NotFound($"Account with id {accountId} does not exist!");
    }

    public async Task<Result<Account>> GetAccountFromIdAsync(Guid accountId, string ownerId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
            return Result.NotFound($"Account with id {accountId} does not exist");
        account.Transactions = await _accountRepository.GetTransactionsAsync(accountId);
        return account.OwnerId != ownerId ? Result.Forbidden() : Result.Success(account);
    }

    public Result UpdateAccount(Account account)
    {
        _accountRepository.Update(account);
        return Result.Success();
    }

    public async Task<Result<List<AccountDto>>> GetAccountsOfOwnerAsync(string ownerId)
    {
        var accounts = (await _accountRepository.GetUserAccountsAsync(ownerId)).Select(account => account.ToAccountDto()).ToList();
        
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
            .FirstOrDefaultAsync(acc => acc.Id.Equals(account.AccountId));

        if (existingAccount is null) return Result.NotFound($"Account with id {account.AccountId} does not exist!");

        if (existingAccount.OwnerId != ownerId) return Result.Forbidden();

        existingAccount.Name = account.Name;
        if (existingAccount.MoneyCurrency != account.MoneyCurrency && account.MoneyBalance is null)
        {
            /* existingAccount.MoneyBalance = existingAccount.MoneyBalance / Utils.GetConversionRate(existingAccount.MoneyCurrency ,account.MoneyCurrency);*/
        }
        else if (account.MoneyBalance.HasValue)
        {
            existingAccount.MoneyBalance = account.MoneyBalance.Value;
        }

        existingAccount.MoneyCurrency = account.MoneyCurrency;
        existingAccount.AccountType = account.AccountType;

        if (existingAccount.Transactions is not null && existingAccount.Transactions.Count > 0)
            foreach (var transaction in existingAccount.Transactions)
                transaction.MoneyCurrency = existingAccount.MoneyCurrency;
        /* transaction.MoneyValue /= Utils.GetConversionRate(existingAccount.MoneyCurrency ,account.MoneyCurrency);*/
        _accountRepository.Update(existingAccount);
        return Result.Success();
    }
}