namespace ProductApi.Domain.Events;

public record ProductRenamedEvent(int ProductId, string OldName, string NewName) : DomainEventBase;
