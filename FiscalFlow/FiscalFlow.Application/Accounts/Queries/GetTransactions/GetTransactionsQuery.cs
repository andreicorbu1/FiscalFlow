using FiscalFlow.Domain.Entities;
using MediatR;
namespace FiscalFlow.Application.Accounts.Queries.GetTransactions;

public sealed class GetTransactionsQuery : IRequest<IList<Transaction>>
{
    public GetTransactionsQuery(Guid accountId) => AccountId = accountId;

    public Guid AccountId { get; }
}