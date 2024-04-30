using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal sealed class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }

    public Transaction? GetByIdIncludingAccount(Guid transactionId)
    {
        return _context.Transactions.Include(tr => tr.Account)
            .FirstOrDefault(transaction => transaction.Id.Equals(transactionId));
    }

    public async Task<Transaction?> GetByIdIncludingAccountAsync(Guid transactionId)
    {
        return await _context.Transactions.Include(tr => tr.Account)
            .FirstOrDefaultAsync(transaction => transaction.Id.Equals(transactionId));
    }

    public async Task<IList<Transaction>> GetTransactionsFromAccountInPeriodOfTime(Guid accountId,
        DateTime startTime, DateTime endTime)
    {
        return await _context.Transactions
            .Where(tr => tr.AccountId.Equals(accountId) && tr.CreatedOnUtc >= startTime && tr.CreatedOnUtc <= endTime)
            .ToListAsync();
    }
}