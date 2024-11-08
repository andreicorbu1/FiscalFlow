﻿using Ardalis.Result;
using FiscalFlow.Contracts.Accounts;
using FiscalFlow.Contracts.Transactions;
using FiscalFlow.Domain.Entities;

namespace FiscalFlow.Application.Core.Abstractions.Services;

public interface ITransactionService
{
    public Task<Result<Transaction>> AddTransaction(AddTransactionRequest payload, string ownerId);
    public Task<Result> UpdateTransaction(UpdateTransaction payload, string ownerId);
    public Task<Result<IList<Transaction>>> GetTransactionsFromAccountPeriodOfTime(string ownerId, Guid accountId,
        PeriodOfTimeRequest period);
    public Task<Result<IList<RecursiveTransactionDto>>> GetSubscriptions(string ownerId, Guid? accountId = null);
    public Result DeleteTransaction(string ownerId, Guid transactionId);
    public Result DeleteRecursiveTransaction(string ownerId, Guid recursiveTransactionId);
}