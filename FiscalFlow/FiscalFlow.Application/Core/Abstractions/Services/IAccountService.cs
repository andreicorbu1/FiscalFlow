using Ardalis.Result;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface IAccountService
{
    Result CheckAccountExistsAndHasAccess(Guid accountId, string ownerId);
    Task<Result> ExportTransactionsAsCsvAsync(Guid accountId, string ownerId);
    Result CreateAccount(CreateAccountRequest payload);
    Result InsertBulkAccounts();
    Result UpdateAccount(Account account);
    Task<Result> UpdateAccount(string ownerId, UpdateAccountRequest account);
    Task<Result<List<AccountDto>>> GetAccountsOfOwnerAsync(string ownerId);
    Task<Result<Account>> GetAccountFromIdAsync(Guid accountId, string ownerId);
    Task<Result<IList<Transaction>>> GetLastTransactions(string ownerId, int numberOfTransactions);
    Result DeleteAccount(Guid accountId, string ownerId);
}