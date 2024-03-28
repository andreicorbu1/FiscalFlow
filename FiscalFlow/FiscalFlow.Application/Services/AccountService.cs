using Ardalis.Result;
using FiscalFlow.Application.Core.Abstractions.Authentication;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Application.Tools.Csv;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Web;

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

    public async Task ExportTransactionsAsCsvAsync(Guid accountId)
    {
        var transactions = await _accountRepository.GetTransactionsAsync(accountId);
        CsvExporter.ExportList(transactions, $"{accountId}");
    }

    public async Task<Result<Account>> GetAccountFromId(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if(account is null)
        {
            return Result.NotFound($"Account with id {accountId} does not exist");
        }
        return Result.Success(account);
    }

    public Result UpdateAccount(Account account)
    {
        _accountRepository.Update(account);
        return Result.Success();
    }
}
