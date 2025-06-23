using ProductApi.Domain.Events;

namespace ProductApi.Domain.Entities;

public abstract class BaseEntity
{
	public required string CreatedBy { get; set; }
	private DateTime _createdOn = DateTime.UtcNow;
	public DateTime CreatedOn
	{
		get => _createdOn;
		set => _createdOn = value == default ? DateTime.UtcNow : value;
	}
	public string? UpdatedBy { get; set; }
	public DateTime? UpdatedOn { get; set; }

	private readonly List<IDomainEvent> _events = [];
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _events;

	protected void AddDomainEvent(IDomainEvent ev) => _events.Add(ev);
	public void ClearDomainEvents() => _events.Clear();
}
