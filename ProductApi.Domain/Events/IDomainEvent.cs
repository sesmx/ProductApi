﻿namespace ProductApi.Domain.Events;

public interface IDomainEvent
{
	DateTime OccurredOn { get; }
}
