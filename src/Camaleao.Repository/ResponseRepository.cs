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
    public class ResponseRepository : IResponseRepository
    {
        readonly MongoContext mongoContext;
        const string ResponseCollection = "response";


        private IMongoCollection<Response> GetMongoCollection()
        {
            return mongoContext.GetCollection<Response>(ResponseCollection);
        }
        public ResponseRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }

        Task IResponseRepository.Add(IEnumerable<Response> responses)
        {
            return GetMongoCollection().InsertManyAsync(responses);
        }

        public Task Add(Response value)
        {
            return GetMongoCollection().InsertOneAsync(value);
        }

        public bool Remove(string id)
        {
            DeleteResult actionResult = GetMongoCollection().DeleteOne(p => p.ResponseId == id);
            return actionResult.IsAcknowledged
                            && actionResult.DeletedCount > 0;
        }

        public bool Update(string id, Response valeu)
        {
            throw new NotImplementedException();
        }

        Task<List<Response>> IRepository<Response>.GetAll()
        {
            return GetMongoCollection().Find(_ => true).ToListAsync();
        }

        Task<List<Response>> IRepository<Response>.Get(Expression<Func<Response, bool>> expression)
        {
            return GetMongoCollection().Find(expression).ToListAsync();
        }
    }
}
