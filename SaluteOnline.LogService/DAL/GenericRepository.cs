using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Extensions;

namespace SaluteOnline.LogService.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity: class, IMongoEntity
    {
        private readonly IMongoDatabase _mongoDb;

        public GenericRepository(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _mongoDb = new MongoClient(configuration.GetValue<string>("MongoSettings:Path"))
                   .GetDatabase(configuration.GetValue<string>("MongoSettings:DB"));
        }

        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new InvalidOperationException("Unable to add a null entity to the repository.");
            _mongoDb.GetCollection<TEntity>(typeof(TEntity).ToMongoCollectionName()).InsertOne(entity);
        }
    }
}