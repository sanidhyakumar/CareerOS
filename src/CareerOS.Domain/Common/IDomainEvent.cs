namespace CareerOS.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

