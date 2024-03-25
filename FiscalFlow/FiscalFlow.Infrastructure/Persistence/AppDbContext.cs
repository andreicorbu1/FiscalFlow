using FiscalFlow.Application.Core.Abstractions.Data;
using FiscalFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiscalFlow.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AppUser>()
            .HasMany(e => e.Accounts)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired();
        modelBuilder.Entity<Account>()
            .HasMany(e => e.Transactions)
            .WithOne(e => e.Account)
            .HasForeignKey(e => e.AccountId)
            .IsRequired();

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountType)
            .HasConversion<string>();
        modelBuilder.Entity<Account>()
            .Property(a => a.MoneyCurrency)
            .HasConversion<string>();
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Category)
            .HasConversion<string>();
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();
        modelBuilder.Entity<Transaction>()
            .Property(t => t.MoneyCurrency)
            .HasConversion<string>();
    }
}
