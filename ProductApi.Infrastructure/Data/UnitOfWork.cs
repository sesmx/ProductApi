using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data.Repositories;

namespace ProductApi.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
	private readonly ApplicationDbContext _db;

	public UnitOfWork(ApplicationDbContext db)
	{
		_db = db;
		Products = new GenericRepository<Product>(db);
		Items = new GenericRepository<Item>(db);
	}

	public IGenericRepository<Product> Products { get; }
	public IGenericRepository<Item> Items { get; }

	public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);

	public ValueTask DisposeAsync() => _db.DisposeAsync();
}
