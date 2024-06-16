using Ardalis.Result;
using FiscalFlow.Contracts;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface IAccountService
{
    Result CheckAccountExistsAndHasAccess(Guid accountId, string ownerId);
    Task<Result<MemoryStream>> ExportTransactionsAsCsvAsync(Guid accountId, string ownerId);
    Task<Result> ImportTransactionsFromCsv(IList<Transaction> transactions, string ownerId, Guid accountId);
    Task<Result<Account>> GetAccountFromNameAndOwner(string ownerId, string accountName);
    Result CreateAccount(CreateAccountRequest payload);
    Task<Result<Dictionary<Category, decimal>>> GetCategoryReportsFromAllAccounts(string ownerId);
    Result InsertBulkAccounts();
    Result UpdateAccount(Account account);
    Task<Result> UpdateAccount(string ownerId, UpdateAccountRequest account);
    Task<Result<List<AccountDto>>> GetAccountsOfOwnerAsync(string ownerId);
    Task<Result<Account>> GetAccountFromIdAsync(Guid accountId, string ownerId);
    Task<Result<List<TransactionDto>>> GetLastTransactions(string ownerId, int numberOfTransactions);
    Result DeleteAccount(Guid accountId, string ownerId);
}