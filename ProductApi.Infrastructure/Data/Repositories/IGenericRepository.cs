using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Data.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
	Task<TEntity?> GetAsync(int id, CancellationToken ct = default);
	Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
	Task AddAsync(TEntity entity, CancellationToken ct = default);
	void Remove(TEntity entity);
}
