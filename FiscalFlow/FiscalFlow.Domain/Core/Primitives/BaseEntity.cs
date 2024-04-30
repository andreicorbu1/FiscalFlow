using System.ComponentModel.DataAnnotations;
using FiscalFlow.Domain.Core.Abstractions;

namespace FiscalFlow.Domain.Core.Primitives;

public abstract class BaseEntity : IAuditableEntity
{
    public Guid Id { get; set; }

    public override bool Equals(object? obj)
    {
        var other = obj as BaseEntity;

        if (ReferenceEquals(other, null)) return false;

        if (ReferenceEquals(this, other)) return true;

        if (GetType() != other.GetType())
            return false;

        if (Id == default || other.Id == default)
            return false;

        return Id == other.Id;
    }

    public static bool operator ==(BaseEntity a, BaseEntity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

        return a.Equals(b);
    }

    public static bool operator !=(BaseEntity a, BaseEntity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public DateTime CreatedOnUtc { get; init; } = DateTime.Now;
    public DateTime? ModifiedOnUtc { get; set; }
}