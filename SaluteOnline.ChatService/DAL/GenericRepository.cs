using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Extensions;

namespace SaluteOnline.ChatService.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity: class, IMongoEntity
    {
        private readonly IMongoDatabase _mongoDb;

        public GenericRepository(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _mongoDb = new MongoClient(configuration.GetValue<string>("MongoSettings:Path"))
                   .GetDatabase(configuration.GetValue<string>("MongoSettings:DB"));
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {
            var collection = filter == null
                    ? _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName()).Find(_ => true).ToList()
                    : _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                        .Find(Builders<TEntity>.Filter.Where(filter))
                        .ToList();
            return orderBy?.Invoke(collection.AsQueryable()).ToList() ?? collection;
        }

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {

            var type = typeof(TEntity).ToMongoCollectionName();
            var collection = filter == null
                ? _mongoDb.GetCollection<TEntity>(type).Find(_ => true)
                : _mongoDb.GetCollection<TEntity>(type).Find(Builders<TEntity>.Filter.Where(filter));
            return await collection.ToListAsync();
        }

        public IQueryable<TEntity> GetAsQueryable(Expression<Func<TEntity, bool>> filter = null)
        {
            var type = typeof(TEntity).ToMongoCollectionName();
            var collection = filter == null
                ? _mongoDb.GetCollection<TEntity>(type).AsQueryable()
                : _mongoDb.GetCollection<TEntity>(type).AsQueryable().Where(filter);
            return collection;
        }

        public Page<TEntity> GetPage(int page, int items, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {
            var type = typeof(TEntity).ToMongoCollectionName();
            var allCollection = filter == null
                ? _mongoDb.GetCollection<TEntity>(type).Find(_ => true)
                : _mongoDb.GetCollection<TEntity>(type)
                    .Find(Builders<TEntity>.Filter.Where(filter));
            var collection = allCollection.SortBy(t => orderBy).Skip((page - 1) * items).Limit(items);
            return new Page<TEntity>(page, (int)collection.Count(), allCollection.Count(), collection.ToList());
        }

        public async Task<Page<TEntity>> GetPageAsync(int page, int items, Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "", bool ascending = false)
        {
            var type = typeof(TEntity).ToMongoCollectionName();
            var allCollection = filter == null
                ? await _mongoDb.GetCollection<TEntity>(type).Find(_ => true).ToListAsync()
                : await _mongoDb.GetCollection<TEntity>(type)
                    .Find(Builders<TEntity>.Filter.Where(filter)).ToListAsync();
            var collection = allCollection.OrderBy(t => orderBy).Skip((page - 1) * items).Take(items).ToList();
            return new Page<TEntity>(page, collection.Count, allCollection.Count, collection.ToList());
        }

        public TEntity GetById(Guid guid)
        {
            return guid != Guid.Empty ? _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                    .Aggregate()
                    .Match(t => t.Guid == guid)
                    .SingleOrDefault() : null;
        }

        public async Task<TEntity> GetByIdAsync(Guid guid)
        {
            return guid != Guid.Empty ? await _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                .Aggregate()
                .Match(t => t.Guid == guid)
                .FirstOrDefaultAsync() : null;
        }

        public void Insert(TEntity entity)
        {
            if (entity == null) throw new InvalidOperationException("Unable to add a null entity to the repository.");
            _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName()).InsertOne(entity);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new InvalidOperationException("Unable to add a null entity to the repository.");
            await _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName()).InsertOneAsync(entity);
            return entity;
        }

        public void Delete(Guid guid)
        {
            if (guid != default(Guid))
            {
                _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
               .FindOneAndDelete(t => t.Guid == guid);
            }
        }

        public async void DeleteAsync(Guid guid)
        {
            await _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                   .FindOneAndDeleteAsync(t => t.Guid == guid);
        }

        public void Delete(TEntity entityToDelete)
        {
            _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                .FindOneAndDelete(t => t.Guid == entityToDelete.Guid);
        }

        public async void DeleteAsync(TEntity entityToDelete)
        {
            await _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                .FindOneAndDeleteAsync(t => t.Guid == entityToDelete.Guid);
        }

        public TEntity Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new InvalidOperationException("Unable to update a null entity in the repository.");
            var result = _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                    .ReplaceOne(t => t.Guid == entityToUpdate.Guid,
                        entityToUpdate);
            return result.IsAcknowledged ? entityToUpdate : null;
        }

        public async Task<TEntity> UpdateAsync(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new InvalidOperationException("Unable to update a null entity in the repository.");
            var result = await _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                .ReplaceOneAsync(t => t.Guid == entityToUpdate.Guid,
                    entityToUpdate);
            return result.IsAcknowledged ? entityToUpdate : null;
        }

        public int Count(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            return Convert.ToInt32(_mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                        .Count(filter ?? Builders<TEntity>.Filter.Empty));
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            return Convert.ToInt32(await _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName())
                        .CountAsync(filter));
        }
    }
}
