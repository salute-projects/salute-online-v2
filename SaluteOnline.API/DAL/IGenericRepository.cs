using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DAL
{
    public interface IGenericRepository<TEntity>
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false);

        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false);

        IQueryable<TEntity> GetAsQueryable(Expression<Func<TEntity, bool>> filter = null);
            
        Page<TEntity> GetPage(int page, int items, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false);

        TEntity GetById(Guid guid = default(Guid), int? id = null);

        Task<TEntity> GetByIdAsync(Guid guid = default(Guid), int? id = null);

        void Insert(TEntity entity);

        void Delete(Guid guid = default(Guid), int? id = null);

        void Delete(TEntity entityToDelete);

        TEntity Update(TEntity entityToUpdate);

        int Count(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
    }
}