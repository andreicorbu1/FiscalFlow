﻿using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal sealed class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context)
    {
    }

    public bool CheckAccountExists(Guid accountId)
    {
        return Any(acc => acc.Id.Equals(accountId));
    }

    public bool CheckIfIsAccountOwner(Guid accountId, string ownerId)
    {
        return Any(acc => acc.Id.Equals(accountId) && acc.UserId.Equals(ownerId));
    }

    public IList<Transaction> GetTransactions(Guid accountId)
    {
        return _context.Accounts
            .Include(account => account.Transactions!)
            .ThenInclude(tr => tr.RecursiveTransaction)
            .Single(account => account.Id.Equals(accountId))
            .Transactions?
            .OrderByDescending(tr => tr.CreatedOnUtc).ToList() ?? new List<Transaction>();
    }

    public async Task<IList<Transaction>> GetTransactionsAsync(Guid accountId)
    {
        return (await _context.Accounts
                .Include(account => account.Transactions!)
                .ThenInclude(tr => tr.RecursiveTransaction)
                .SingleOrDefaultAsync(account => account.Id.Equals(accountId)))?
            .Transactions?.OrderByDescending(tr => tr.CreatedOnUtc).ToList() ?? new List<Transaction>();
    }

    public async Task<IReadOnlyCollection<Account>> GetUserAccountsAsync(string userId)
    {
        return await _context.Accounts
        .Include(acc => acc.Transactions!)
        .ThenInclude(tr => tr.RecursiveTransaction)
        .Where(account => account.UserId == userId)
        .Select(account => new Account
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,
            MoneyBalance = account.MoneyBalance,
            MoneyCurrency = account.MoneyCurrency,
            AccountType = account.AccountType,
            Transactions = account.Transactions!.OrderByDescending(t => t.CreatedOnUtc).ToList()
        })
        .OrderBy(acc => acc.Name)
        .ToListAsync();
    }

    public async Task<IList<Transaction>> GetLastTransactionsAsync(string userId, int numberOfTransactions)
    {
        return await _context.Accounts
            .Where(account => account.UserId == userId)
            .Include(account => account.Transactions!)
            .ThenInclude(transaction => transaction.Account)
            .Include(account => account.Transactions!)
            .ThenInclude(transaction => transaction.RecursiveTransaction)
            .SelectMany(ac => ac.Transactions!)
            .OrderByDescending(tr => tr.CreatedOnUtc)
            .Take(numberOfTransactions)
            .ToListAsync();
    }

    public bool CheckAccountWithSameName(string ownerId, string accoutName)
    {
        return _context.Accounts.Any(ac => ac.Name == accoutName && ac.UserId == ownerId);
    }

    public async Task<Account?> GetAccountFromAccountNameAnDOwnerId(string accountName, string ownerId)
    {
        return await _context.Accounts.FirstOrDefaultAsync(ac => ac.Name == accountName && ac.UserId == ownerId);
    }
}