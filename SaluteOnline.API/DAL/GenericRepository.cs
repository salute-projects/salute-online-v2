using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity: class, IEntity
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly DbContext _context;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {
            var query = GetGeneric(filter, includeProperties, orderBy, ascending);
            return orderBy?.Invoke(query).ToList() ?? query.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {
            var query = GetGeneric(filter, includeProperties, orderBy, ascending);
            if (orderBy != null)
                return await orderBy.Invoke(query).ToListAsync();
            return await query.ToListAsync();
        }

        public IQueryable<TEntity> GetAsQueryable(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter != null ? _dbSet.Where(filter) : _dbSet;
        }

        public Page<TEntity> GetPage(int page, int items, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {
            var needed = GetGeneric(filter, includeProperties, orderBy, ascending);
            var all = needed.Count();
            var slice = needed.Skip((page - 1) * items).Take(items).ToList();
            return new Page<TEntity>(page, items, all, slice);
        }

        public TEntity GetById(Guid guid = default(Guid), int? id = null)
        {
            return id != null ? _dbSet.SingleOrDefault(t => t.Id == id) : _dbSet.SingleOrDefault(t => t.Guid == guid);
        }

        public async Task<TEntity> GetByIdAsync(Guid guid = default(Guid), int? id = null)
        {
            return id != null ? await _dbSet.SingleOrDefaultAsync(t => t.Id == id) : await _dbSet.SingleOrDefaultAsync(t => t.Guid == guid);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new InvalidOperationException("Unable to add a null entity to the repository.");
            _dbSet.Add(entity);
        }

        public void Delete(Guid guid = default(Guid), int? id = null)
        {
            var entityToDelete = id != null ? _dbSet.SingleOrDefault(t => t.Id == id) : _dbSet.SingleOrDefault(t => t.Guid == guid);
            if (entityToDelete == null)
                throw new InvalidOperationException("Unable to delete a null entity.");
            Delete(entityToDelete);
        }

        public async void DeleteAsync(Guid guid = default(Guid), int? id = null)
        {
            var entityToDelete = id != null ? await _dbSet.SingleOrDefaultAsync(t => t.Id == id) : await _dbSet.SingleOrDefaultAsync(t => t.Guid == guid);
            if (entityToDelete == null)
                throw new InvalidOperationException("Unable to delete a null entity.");
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public TEntity Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new InvalidOperationException("Unable to update a null entity in the repository.");

            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return entityToUpdate;
        }

       public int Count(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            var query = GetGeneric(filter, includeProperties);
            return query.Count();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            var query = GetGeneric(filter, includeProperties);
            return await query.CountAsync();
        }

        private IQueryable<TEntity> GetGeneric(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "", Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, bool ascending = false)
        {
            var query = filter != null ? _dbSet.Where(filter) : _dbSet;
            var navProperties = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (navProperties.Length > 0)
            {
                query = navProperties.Aggregate(query, (current, nav) => current.Include(nav));
            }
            if (orderBy == null)
                return query;
            return ascending ? query.OrderByDescending(t => orderBy) : query.OrderBy(t => orderBy);
        }
    }
}
