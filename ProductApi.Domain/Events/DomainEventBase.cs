namespace ProductApi.Domain.Events;

public abstract record DomainEventBase(DateTime OccurredOn) : IDomainEvent
{
	protected DomainEventBase() : this(DateTime.UtcNow) { }
}
