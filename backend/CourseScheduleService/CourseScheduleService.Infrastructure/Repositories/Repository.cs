using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Repositories
{
  public class Repository<T> : IRepository<T> where T : class
  {
    private readonly CourseScheduleDbContext _context;
    protected readonly DbSet<T> _dbSet;

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
      return await _dbSet.Where(x => !EF.Property<bool>(x, "IsDeleted")).ToArrayAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
      return await _dbSet.FirstOrDefaultAsync(x =>
            EF.Property<int>(x, "Id") == id &&
            !EF.Property<bool>(x, "IsDeleted"));
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
    {
      return await _dbSet
        .Where(x => !EF.Property<bool>(x, "IsDeleted"))
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

    public async Task<(IEnumerable<T> Data, int TotalRecords)> GetPagedAsync(
      int page,
      int pageSize,
      Expression<Func<T, bool>>? filter = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    ) {
      var query = _dbSet.Where(x => !EF.Property<bool>(x, "IsDeleted"));

      if (filter != null)
        query = query.Where(filter);

      var totalRecords = await query.CountAsync();

      if (orderBy != null)
        query = orderBy(query);

      var data = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToArrayAsync();

      return (data, totalRecords);
    }
  }
}