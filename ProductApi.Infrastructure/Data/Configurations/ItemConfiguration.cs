using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
	public void Configure(EntityTypeBuilder<Item> b)
	{
		b.ToTable("Items");

		b.HasKey(i => i.Id);

		b.Property(i => i.Quantity)
		 .IsRequired();

		// index for faster look-ups by product
		b.HasIndex(i => i.ProductId);

		b.HasOne(i => i.Product)
		 .WithMany(p => p.Items)
		 .HasForeignKey(i => i.ProductId);
	}
}
