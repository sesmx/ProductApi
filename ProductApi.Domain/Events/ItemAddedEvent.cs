namespace ProductApi.Domain.Events;

public record ItemAddedEvent(int ProductId, int ItemId, int Quantity) : DomainEventBase;
