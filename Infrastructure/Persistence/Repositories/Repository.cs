using Application.Abstractions;
using Application.Common;
using Domain.Common;
using Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AutoTallerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AutoTallerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalItems, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        var isActiveProperty = entity.GetType().GetProperty("IsActive");
        if (isActiveProperty != null && isActiveProperty.CanWrite)
        {
            isActiveProperty.SetValue(entity, false);
            Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
    }
}