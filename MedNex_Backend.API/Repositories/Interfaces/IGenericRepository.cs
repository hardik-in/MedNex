using MedNex_Backend.API.DTOs.Common;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace MedNex_Backend.API.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByPublicIdAsync(Guid publicId);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);
        Task SoftDeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<int> GetYearlyCountAsync(int year);
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            PagedRequest request,
            Expression<Func<T, bool>>? filter = null);
    }
}