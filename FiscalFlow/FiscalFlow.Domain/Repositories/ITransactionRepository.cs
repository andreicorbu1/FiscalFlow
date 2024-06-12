using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Transaction? GetByIdIncludingAccount(Guid transactionId);
    Transaction? GetByIdIncludingAccountReccursiveTransaction(Guid transactionId);

    Task<Transaction?> GetByIdIncludingAccountAsync(Guid transactionId);

    Task<IList<Transaction>> GetTransactionsFromAccountInPeriodOfTime(Guid accountId, DateTime startTime,
        DateTime endTime);
}