using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IRepository<TEntity> : IDisposable
    {
        IQueryable<TEntity> Get();
        ValueTask<TEntity> FindAsync(CancellationToken cancellationToken = default, params object[] key);
        void AddRange(TEntity obj);
        Task<TEntity> AddAsync(TEntity obj, CancellationToken cancellationToken = default);
        void UpdateRange(TEntity obj);
        Task<TEntity> UpdateAsync(TEntity obj, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> list, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> list, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity obj, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        void RemoveRange(params TEntity[] entities);
        Task<int> SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}