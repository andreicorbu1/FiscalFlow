using Ardalis.Result;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface IAccountService
{
    Task ExportTransactionsAsCsvAsync(Guid accountId);
    Result CreateAccount(CreateAccountRequest account);
    Result UpdateAccount(Account account);
    Task<Result<Account>> GetAccountFromId(Guid accountId);
}
