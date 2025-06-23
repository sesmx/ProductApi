namespace ProductApi.Domain.Events;

public record ProductCreatedEvent(int ProductId, string Name) : DomainEventBase;
