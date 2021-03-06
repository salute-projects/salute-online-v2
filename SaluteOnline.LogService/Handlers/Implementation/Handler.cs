﻿using SaluteOnline.LogService.DAL;
using SaluteOnline.LogService.Handlers.Abstraction;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.LogService.Handlers.Implementation
{
    public class Handler<TEntity> : IHandler<TEntity> where TEntity: class, IMongoEntity
    {
        private readonly IGenericRepository<TEntity> _repository;

        public Handler(IGenericRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual void HandleAndInsert<T>(TEntity message)
        {
            _repository.Insert(message);
        }
    }
}
