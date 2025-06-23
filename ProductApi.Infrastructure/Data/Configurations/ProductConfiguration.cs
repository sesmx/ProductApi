using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> b)
	{
		b.ToTable("Products");

		// Primary-key
		b.HasKey(p => p.Id);

		// Columns
		b.Property(p => p.ProductName)
		 .IsRequired()
		 .HasMaxLength(255);

		// Relationships
		b.HasMany(p => p.Items)
		 .WithOne(i => i.Product)
		 .HasForeignKey(i => i.ProductId)
		 .OnDelete(DeleteBehavior.Cascade);
	}
}
