using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal sealed class RecursiveTransactionRepository : GenericRepository<RecursiveTransaction>, IRecursiveTransactionRepository
{
    public RecursiveTransactionRepository(AppDbContext context) : base(context)
    {
    }
    public RecursiveTransaction? GetByIdIncludingTransaction(Guid id)
    {
        var rt = _context.RecursiveTransactions.Include(rt => rt.Transactions).FirstOrDefault(rt => rt.Id == id);
        return rt;
    }
    public async Task<IList<RecursiveTransaction>> GetRecursiveTransactions(string ownerId, Guid? accountId = null)
    {
        if (accountId is null)
        {
            return await _context.RecursiveTransactions.Include(rt => rt.Transactions)
                .ThenInclude(rt => rt.Account)
                .Where(rt => rt.Recurrence > 0 && rt.OwnerId == ownerId)
                .ToListAsync();
        }
        return await _context.RecursiveTransactions.Include(rt => rt.Transactions)
                .ThenInclude(tr => tr.Account)
                .Where(rt => rt.Recurrence > 0 && rt.OwnerId == ownerId && rt.Transactions.First().AccountId == accountId)
                .ToListAsync();
    }
}
