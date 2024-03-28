using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface IAccountRepository : IGenericRepository<Account>
{
    IList<Transaction> GetTransactions(Guid accountId);
    Task<IList<Transaction>> GetTransactionsAsync(Guid accountId);
}