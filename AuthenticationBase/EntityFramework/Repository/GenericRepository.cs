using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthenticationBase.EntityFramework.Repository;

public sealed class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _table;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
        _table = _context.Set<T>();
    }

    public async Task Add(T entity)
    {
        await _table.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        _table.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public IQueryable<T> GetAll() => _table.AsNoTracking();

    public Task<T?> GetBy(Expression<Func<T, bool>> predicate) => _table.FirstOrDefaultAsync(predicate);

    public async Task<T?> GetById(Guid id) =>  await _table.FindAsync(id);

    public async Task Update(T entity)
    {
        _table.Update(entity);
        await _context.SaveChangesAsync();
    }
}