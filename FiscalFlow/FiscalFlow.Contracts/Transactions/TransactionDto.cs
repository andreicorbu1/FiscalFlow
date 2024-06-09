using FiscalFlow.Domain.Enums;

namespace FiscalFlow.Contracts.Accounts;

public class TransactionDto
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Currency { get; set; }
    public decimal AccountValueBefore { get; set; }
    public decimal AccountValueAfter { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Payee { get; set; }
    public TransactionType Type { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Category Category { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public ushort? ReccurencePeriod { get; set; } = null;
    public string Account { get; set; }
}