using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal sealed class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context)
    {
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
                .SingleAsync(account => account.Id.Equals(accountId)))
            .Transactions ?? new List<Transaction>();
    }
}