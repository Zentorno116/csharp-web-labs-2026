using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.Repositories.Interfaces;

namespace TaskFlowApi.Repositories.Implementations;

public class GenericRepository<T>(TaskFlowDbContext context) : IGenericRepository<T> where T : class
{
    protected readonly TaskFlowDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);
    public async Task<List<T>> GetAllAsync() => await DbSet.ToListAsync();
    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) => await DbSet.Where(predicate).ToListAsync();
    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);
    public void Update(T entity) => DbSet.Update(entity);
    public void Remove(T entity) => DbSet.Remove(entity);
}
