using Ardalis.Result;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface IAccountService
{
    Task<Result> ExportTransactionsAsCsvAsync(Guid accountId, string ownerId);
    Result CreateAccount(CreateAccountRequest payload);
    Result InsertBulkAccounts();
    Result UpdateAccount(Account account);
    Task<Result> UpdateAccount(string ownerId, UpdateAccountRequest account);
    Task<Result<IReadOnlyCollection<Account>>> GetAccountsOfOwnerAsync(string ownerId);
    Task<Result<Account>> GetAccountFromIdAsync(Guid accountId, string ownerId);
    Result DeleteAccount(Guid accountId, string ownerId);
}
