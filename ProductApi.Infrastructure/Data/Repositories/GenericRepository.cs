using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Data.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
	private readonly ApplicationDbContext _db;
	private readonly DbSet<TEntity> _set;

	public GenericRepository(ApplicationDbContext db)
	{
		_db = db;
		_set = db.Set<TEntity>();
	}

	public async Task<TEntity?> GetAsync(int id, CancellationToken ct = default) 
		=> await _set.FindAsync(new object?[] { id }, ct);

	public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
		=> predicate is null
			  ? await _set.AsNoTracking().ToListAsync(ct)
			  : await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

	public Task AddAsync(TEntity entity, CancellationToken ct = default)
		=> _set.AddAsync(entity, ct).AsTask();

	public void Remove(TEntity entity) => _set.Remove(entity);
}
