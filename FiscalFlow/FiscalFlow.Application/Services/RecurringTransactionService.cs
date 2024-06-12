using FiscalFlow.Application.Core.Abstractions.Data;
using FiscalFlow.Application.Core.Abstractions.Services;
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
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessRecurringTransactions();
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessRecurringTransactions()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var now = DateTime.UtcNow;
            var transactions = await dbContext.RecursiveTransactions
                .Include(rt => rt.Transactions)
                    .ThenInclude(t => t.Account)
                .Where(rt => rt.Recurrence > 0 && rt.Transactions.OrderByDescending(t => t.CreatedOnUtc).FirstOrDefault().CreatedOnUtc.AddMonths(1) <= now)
                .ToListAsync();
            //var transactions = await dbContext.RecursiveTransactions
            //    .Include(rt => rt.Transactions)
            //        .ThenInclude(tr => tr.Account)
            //    .ToListAsync();
            //transactions = transactions.Where(rt => rt.Recurrence > 0 && rt.Transactions[-1].CreatedOnUtc.AddMonths(1) <= now).ToList();
            foreach (var rt in transactions)
            {
                var lastTransaction = rt.Transactions[0];
                var newTransaction = new Contracts.Transactions.AddTransactionRequest
                {
                    AccountId = lastTransaction.AccountId,
                    Value = lastTransaction.MoneyValue,
                    Description = lastTransaction.Description,
                    Payee = lastTransaction.Payee,
                    Latitude = lastTransaction.Latitude,
                    Longitude = lastTransaction.Longitude,
                    IsRecursive = false,
                    Recurrence = null,
                    Type = lastTransaction.Type,
                    Category = lastTransaction.Category,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();
                var newTr = await transactionService.AddTransaction(newTransaction, rt.OwnerId);
                if (!newTr.IsSuccess)
                    continue;
                newTr.Value.ReccursiveTransactionId = rt.Id;
                rt.Transactions.Add(newTr.Value);
                rt.Recurrence--;
                rt.ModifiedOnUtc = DateTime.UtcNow;
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
