using Camaleao.Core.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Camaleao.Repository
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly MongoContext mongoContext;
    
        public BaseRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }

        abstract protected string GetCollectionName();

        protected IMongoCollection<TEntity> GetMongoCollection()
        {
            return mongoContext.GetCollection<TEntity>(GetCollectionName());
        }

        public Task Add(TEntity filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return GetMongoCollection().InsertOneAsync(filter);
        }

        public List<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            return GetMongoCollection().Find(filter).ToListAsync().Result;
        }

        public List<TEntity> GetAll()
        {
            return GetMongoCollection().Find(_ => true).ToListAsync().Result;
        }

        public bool Remove(Expression<Func<TEntity, bool>> filter)
        {
            DeleteResult actionResult = GetMongoCollection().DeleteMany(filter);
            return actionResult.IsAcknowledged
                            && actionResult.DeletedCount > 0;
        }

        public bool Update(Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            return GetMongoCollection().ReplaceOne(filter, entity).IsAcknowledged;
        }
    }
}
