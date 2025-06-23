using ProductApi.Domain.Events;

namespace ProductApi.Domain.Entities;

public class Product : BaseEntity
{
	public int Id { get; set; }
	public string ProductName { get; set; }
	public ICollection<Item> Items { get; set; } = new List<Item>();

	private Product() { }

	public static Product Create(string name, string createdBy)
	{
		var p = new Product
		{
			ProductName = name,
			CreatedBy = createdBy
		};

		p.AddDomainEvent(new ProductCreatedEvent(p.Id, name));
		return p;
	}

	public void Rename(string newName, string updatedBy)
	{
		if (string.Equals(ProductName, newName, StringComparison.Ordinal))
			return;

		var old = ProductName;
		ProductName = newName;
		UpdatedBy = updatedBy;
		UpdatedOn = DateTime.UtcNow;

		AddDomainEvent(new ProductRenamedEvent(Id, old, newName));
	}

	public Item AddItem(int quantity, string createdBy)
	{
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(quantity, 0);

		var item = new Item
		{
			Quantity = quantity,
			CreatedBy = createdBy
		};

		Items.Add(item);
		AddDomainEvent(new ItemAddedEvent(Id, item.Id, quantity));
		return item;
	}
}
