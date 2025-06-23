using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data.Repositories;

namespace ProductApi.Infrastructure.Data;

public interface IUnitOfWork : IAsyncDisposable
{
	IGenericRepository<Product> Products { get; }
	IGenericRepository<Item> Items { get; }

	Task<int> SaveChangesAsync(CancellationToken ct = default);
}
