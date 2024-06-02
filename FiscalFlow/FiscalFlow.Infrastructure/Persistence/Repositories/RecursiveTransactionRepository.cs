using FiscalFlow.Domain.Entities;
using FiscalFlow.Domain.Repositories;

namespace FiscalFlow.Infrastructure.Persistence.Repositories;

internal sealed class RecursiveTransactionRepository : GenericRepository<RecursiveTransaction>, IRecursiveTransactionRepository
{
    public RecursiveTransactionRepository(AppDbContext context) : base(context)
    {
    }
}
