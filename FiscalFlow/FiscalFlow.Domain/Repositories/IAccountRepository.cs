using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface IAccountRepository : IGenericRepository<Account>
{
    IEnumerable<Transaction> GetTransactions(Guid accountId);
    Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid accountId);
}