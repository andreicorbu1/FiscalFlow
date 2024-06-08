using FiscalFlow.Domain.Core.Abstractions;
using FiscalFlow.Domain.Core.Primitives;

namespace FiscalFlow.Domain.Entities;

public class RecursiveTransaction : BaseEntity
{
    public ushort Recurrence { get; set; } = 12;

    public Guid TransactionId { get; set; }
    public Transaction? LastTransaction { get; set; }
    public string OwnerId { get; set; }
    public AppUser? Owner { get; set; }
}
