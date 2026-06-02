using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
  public interface IRepository<T> where T : class
  {
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void UpdateAsync(T entity);
    void DeleteAsync(T entity);
    Task SaveChangeAsync();
    Task<(IEnumerable<T> Data, int TotalRecords)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
    );
  }
}