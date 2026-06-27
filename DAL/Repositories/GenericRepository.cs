using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;

namespace DAL.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.ToListAsync(cancellationToken);
    }

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync([id], cancellationToken).AsTask();
    }

    public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _dbSet.AddAsync(entity, cancellationToken).AsTask();
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}

