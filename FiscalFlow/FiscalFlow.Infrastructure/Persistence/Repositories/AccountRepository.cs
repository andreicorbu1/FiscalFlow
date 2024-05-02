using FiscalFlow.Domain.Entities;
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
        return Any(acc => acc.Id.Equals(accountId) && acc.OwnerId.Equals(ownerId));
    }

    public IList<Transaction> GetTransactions(Guid accountId)
    {
        return _context.Accounts
            .Include(account => account.Transactions)
            .Single(account => account.Id.Equals(accountId))
            .Transactions ?? new List<Transaction>();
    }

    public async Task<IList<Transaction>> GetTransactionsAsync(Guid accountId)
    {
        return (await _context.Accounts
                .Include(account => account.Transactions)
                .SingleOrDefaultAsync(account => account.Id.Equals(accountId)))?
            .Transactions ?? new List<Transaction>();
    }

    public async Task<IReadOnlyCollection<Account>> GetUserAccountsAsync(string userId)
    {
        return await _context.Accounts.Include(acc => acc.Transactions).Where(account => account.OwnerId == userId).ToListAsync();
    }

    public async Task<IList<Transaction>> GetLastTransactionsAsync(string userId, int numberOfTransactions)
    {
        return await _context.Accounts
            .Where(account => account.OwnerId == userId)
            .Include(ac => ac.Transactions!)
            .ThenInclude(tr => tr.Account)
            .SelectMany(ac => ac.Transactions!)
            .OrderByDescending(tr => tr.CreatedOnUtc)
            .Take(numberOfTransactions)
            .ToListAsync();
    }

    public bool CheckAccountWithSameName(string ownerId, string accoutName)
    {
        return _context.Accounts.Any(ac => ac.Name == accoutName && ac.OwnerId == ownerId);
    }
}