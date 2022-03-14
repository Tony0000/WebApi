using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private WebApiContext _context;

        protected Repository(WebApiContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Get()
        {
            return _context.Set<TEntity>();
        }

        public virtual ValueTask<TEntity> FindAsync(CancellationToken cancellationToken, params object[] key)
        {
            return _context.Set<TEntity>().FindAsync(key, cancellationToken);
        }

        public void AddRange(TEntity obj)
        {
            _context.Set<TEntity>().Add(obj);
        }

        public virtual async Task<TEntity> AddAsync(TEntity obj, CancellationToken cancellationToken)
        {
            await _context.Set<TEntity>().AddAsync(obj, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return obj;
        }

        public void UpdateRange(TEntity obj)
        {
            _context.Set<TEntity>().Update(obj);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity obj, CancellationToken cancellationToken)
        {
            var id = obj.Id;
            var model = await FindAsync(cancellationToken, id);
            _context.Entry(model).CurrentValues.SetValues(obj);
            _context.Entry(model).State = EntityState.Modified;
            
            await _context.SaveChangesAsync(cancellationToken);

            return obj;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(
            IEnumerable<TEntity> list, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().UpdateRange(list);
            await _context.SaveChangesAsync(cancellationToken);

            return list;
        }

        public virtual async Task AddRangeAsync(
            IEnumerable<TEntity> list, CancellationToken cancellationToken)
        {
            await _context.Set<TEntity>().AddRangeAsync(list, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var list = await _context.Set<TEntity>()
                .Where(predicate)
                .ToListAsync(cancellationToken);

            _context.Set<TEntity>().RemoveRange(list);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity obj, CancellationToken cancellationToken)
        {
            _context.Set<TEntity>().Remove(obj);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public void RemoveRange(params TEntity[] entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }
        
        public Task<int> SaveChanges()
        {
            return _context.SaveChangesAsync(CancellationToken.None);
        }
        
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposing || _context == null) return;
            _context.Dispose();
            _context = null;
        }
    }
}