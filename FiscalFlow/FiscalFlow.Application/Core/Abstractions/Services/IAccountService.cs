using Ardalis.Result;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface IAccountService
{
    Task ExportTransactionsAsCsvAsync(Guid accountId);
    Result CreateAccount(CreateAccountRequest payload);
    Result InsertBulkAccounts();
    Result UpdateAccount(Account account);
    Task<Result<IReadOnlyCollection<Account>>> GetAccountsOfOwnerAsync(string ownerId);
    Task<Result<Account>> GetAccountFromIdAsync(Guid accountId);
    Result DeleteAccount(Guid accountId);
}
