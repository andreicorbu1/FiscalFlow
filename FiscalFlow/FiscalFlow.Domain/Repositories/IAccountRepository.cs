using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetAccountFromAccountNameAnDOwnerId(string accountName, string ownerId);
    bool CheckAccountExists(Guid accountId);
    bool CheckIfIsAccountOwner(Guid accountId, string ownerId);
    IList<Transaction> GetTransactions(Guid accountId);
    Task<IList<Transaction>> GetTransactionsAsync(Guid accountId);
    Task<IReadOnlyCollection<Account>> GetUserAccountsAsync(string userId);
    Task<IList<Transaction>> GetLastTransactionsAsync(string userId, int numberOfTransactions);
    bool CheckAccountWithSameName(string payloadOwnerId, string payloadName);
}