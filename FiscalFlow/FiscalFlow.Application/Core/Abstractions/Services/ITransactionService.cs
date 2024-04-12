using Ardalis.Result;
using FiscalFlow.Contracts.Transactions;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface ITransactionService
{
    public Task<Result> AddTransaction(AddTransactionRequest payload, string ownerId);
    public Result DeleteTransaction(string ownerId, Guid transactionId);
}