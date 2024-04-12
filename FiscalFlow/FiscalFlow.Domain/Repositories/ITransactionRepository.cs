using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Domain.Repositories;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Transaction? GetByIdIncludingAccount(Guid transactionId);
    Task<Transaction?> GetByIdIncludingAccountAsync(Guid transactionId);
}
