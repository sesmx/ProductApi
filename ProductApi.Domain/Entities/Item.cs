using ProductApi.Domain.Events;

namespace ProductApi.Domain.Entities;

public class Item : BaseEntity
{
	public int Id { get; private set; }
	public int ProductId { get; internal set; }
	public int Quantity { get; set; }

	public Product Product { get; internal set; } = default!;
}
