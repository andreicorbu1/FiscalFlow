using FiscalFlow.Application.Core.Abstractions.Data;
using FiscalFlow.Application.Core.Abstractions.Services;
using FiscalFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FiscalFlow.Application.Services;

public class RecurringTransactionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RecurringTransactionService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            await ProcessRecurringTransactions();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessRecurringTransactions()
    {
        using(var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var now = DateTime.UtcNow;
            var transactions = await dbContext.RecursiveTransactions
                .Include(rt => rt.LastTransaction)
                .Include(rt => rt.LastTransaction.Account)
                .Where(rt => rt.LastTransaction.CreatedOnUtc.AddMonths(1) <= now)
                .ToListAsync();
            foreach (var rt in transactions)
            {
                var newTransaction = new Contracts.Transactions.AddTransactionRequest
                {
                    AccountId = rt.LastTransaction.AccountId,
                    Value = rt.LastTransaction.MoneyValue,
                    Description = rt.LastTransaction.Description,
                    Payee = rt.LastTransaction.Payee,
                    Latitude = rt.LastTransaction.Latitude,
                    Longitude = rt.LastTransaction.Longitude,
                    IsRecursive = false,
                    Recurrence = null,
                    Type = rt.LastTransaction.Type,
                    Category = rt.LastTransaction.Category,
                    CreatedOnUtc = DateTime.UtcNow
                };
                var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                var newTr = await transactionService.AddTransaction(newTransaction, rt.OwnerId);
                if(!newTr.IsSuccess)
                    continue;
                rt.LastTransaction = newTr.Value;
                rt.Recurrence--;
                rt.ModifiedOnUtc = DateTime.UtcNow;
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
