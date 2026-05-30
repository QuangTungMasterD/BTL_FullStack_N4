using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Repositories
{
  public class Repository<T> : IRepository<T> where T : class
  {
    private readonly CourseScheduleDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(CourseScheduleDbContext context)
    {
      this._context = context;
      _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
      await _dbSet.AddAsync(entity);
    }

    public void DeleteAsync(T entity)
    {
      _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _dbSet.ToArrayAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
      return await _dbSet.FindAsync(id);;
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
    {
      return await _dbSet
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    }

    public async Task SaveChangeAsync()
    {
      await _context.SaveChangesAsync();
    }

    public void UpdateAsync(T entity)
    {
      _dbSet.Update(entity);
    }

    public IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }
  }
}