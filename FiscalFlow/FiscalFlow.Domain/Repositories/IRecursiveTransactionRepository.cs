using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface IRecursiveTransactionRepository : IGenericRepository<RecursiveTransaction>
{
    public RecursiveTransaction? GetByIdIncludingTransaction(Guid id);
    public Task<IList<RecursiveTransaction>> GetRecursiveTransactions(string ownerId, Guid? accountId = null);
}
