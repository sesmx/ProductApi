using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Identity;

namespace ProductApi.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public DbSet<Product> Products { get; set; }
	public DbSet<Item> Items { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

		base.OnModelCreating(modelBuilder);
	}

	public override Task<int> SaveChangesAsync(CancellationToken ct = default)
	{
		foreach (var e in ChangeTracker.Entries<BaseEntity>())
		{
			var now = DateTime.UtcNow;
			if (e.State == EntityState.Added) e.Entity.CreatedOn = now;
			else if (e.State == EntityState.Modified) e.Entity.UpdatedOn = now;
		}
		return base.SaveChangesAsync(ct);
	}
}
