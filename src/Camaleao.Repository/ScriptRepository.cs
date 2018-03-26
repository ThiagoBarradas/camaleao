using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Camaleao.Core;
using Camaleao.Core.Repository;
using MongoDB.Driver;

namespace Camaleao.Repository
{
    public class ScriptRepository : IScriptRepository
    {
        readonly MongoContext mongoContext;
        const string ScriptCollection = "Scripts";

        private IMongoCollection<ScriptEngine> GetMongoCollection()
        {
            return mongoContext.GetCollection<ScriptEngine>(ScriptCollection);
        }

        public ScriptRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }
        public Task Add(ScriptEngine value)
        {
            return GetMongoCollection().InsertOneAsync(value);
        }

        public Task<List<ScriptEngine>> Get(Expression<Func<ScriptEngine, bool>> expression)
        {
            return GetMongoCollection().Find(expression).ToListAsync();
        }

        public Task<List<ScriptEngine>> GetAll()
        {
            return GetMongoCollection().Find(_ => true).ToListAsync();
        }

        public bool Remove(string id)
        {
            DeleteResult actionResult = GetMongoCollection().DeleteOne(p => p.Id == Guid.Parse(id));
            return actionResult.IsAcknowledged
                            && actionResult.DeletedCount > 0;
        }

        public bool Update(string id, ScriptEngine valeu)
        {
            throw new NotImplementedException();
        }
    }
}
