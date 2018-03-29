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
    public class CallbackRepository : ICallbackRepository
    {
        readonly MongoContext mongoContext;
        const string CallbackCollection = "callback";


        private IMongoCollection<Callback> GetMongoCollection()
        {
            return mongoContext.GetCollection<Callback>(CallbackCollection);
        }
        public CallbackRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }

        public Task Add(Callback value)
        {
            return GetMongoCollection().InsertOneAsync(value);
        }

        public bool Remove(string id)
        {
            DeleteResult actionResult = GetMongoCollection().DeleteOne(p => p.CID == id);
            return actionResult.IsAcknowledged
                            && actionResult.DeletedCount > 0;
        }

        public bool Update(string id, Callback value)
        {
            return GetMongoCollection().ReplaceOne(x => x.CID == id, value).IsAcknowledged;
        }

        Task<List<Callback>> IRepository<Callback>.GetAll()
        {
            return GetMongoCollection().Find(_ => true).ToListAsync();
        }

        Task<List<Callback>> IRepository<Callback>.Get(Expression<Func<Callback, bool>> expression)
        {
            return GetMongoCollection().Find(expression).ToListAsync();
        }
    }
}
