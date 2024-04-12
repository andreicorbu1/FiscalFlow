using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface IAccountRepository : IGenericRepository<Account>
{
    bool CheckAccountExists(Guid accountId);
    bool CheckIfIsAccountOwner(Guid accountId, string ownerId);
    IList<Transaction> GetTransactions(Guid accountId);
    Task<IList<Transaction>> GetTransactionsAsync(Guid accountId);
    Task<IReadOnlyCollection<Account>> GetUserAccountsAsync(string userId);
}