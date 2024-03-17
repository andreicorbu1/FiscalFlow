using FiscalFlow.Domain.Entities;
using MediatR;

namespace FiscalFlow.Application.Accounts.Queries.GetTransactions;

internal sealed class GetTransactionQueryHandler : IRequestHandler<GetTransactionsQuery, IList<Transaction>>
{
    // protected readonly 
    public Task<IList<Transaction>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}