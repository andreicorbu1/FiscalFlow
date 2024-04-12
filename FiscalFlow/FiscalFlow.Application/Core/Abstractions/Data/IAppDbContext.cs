using FiscalFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Application.Core.Abstractions.Data;

public interface IAppDbContext
{
    public DbSet<AppUser> Users { get; }
    public DbSet<Account> Accounts { get; }
    public DbSet<Transaction> Transactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}