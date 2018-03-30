using Camaleao.Core;
using Camaleao.Core.Repository;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Repository
{
    public class ContextRepository : IContextRepository
    {
        readonly MongoContext mongoContext;
        const string ContextCollection = "context";


        private IMongoCollection<Context> GetMongoCollection()
        {
            return mongoContext.GetCollection<Context>(ContextCollection);
        }
        public ContextRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }

        public Task Add(Context value)
        {
            return GetMongoCollection().InsertOneAsync(value);
        }

        public bool Remove(string id)
        {
            DeleteResult actionResult = GetMongoCollection().DeleteOne(p => p.Id.ToString() == id);
            return actionResult.IsAcknowledged
                            && actionResult.DeletedCount > 0;
        }

        public bool Update(string id, Context value)
        {
            return GetMongoCollection().ReplaceOne(x => x.Id == Guid.Parse(id), value).IsAcknowledged;
        }

        Task<List<Context>> IRepository<Context>.GetAll()
        {
            return GetMongoCollection().Find(_ => true).ToListAsync();
        }

        Task<List<Context>> IRepository<Context>.Get(Expression<Func<Context, bool>> expression)
        {
            return GetMongoCollection().Find(expression).ToListAsync();
        }
    }
}
