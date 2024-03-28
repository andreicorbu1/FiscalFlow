using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal sealed class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {
    }
}
