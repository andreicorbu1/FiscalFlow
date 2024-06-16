using FiscalFlow.Domain.Core.Primitives;

namespace FiscalFlow.Domain.Entities;

public class RecursiveTransaction : BaseEntity
{
    public ushort Recurrence { get; set; }

    public IList<Transaction> Transactions { get; set; } = new List<Transaction>();
    public string UserId { get; set; }
    public AppUser? User { get; set; }
}
