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
    public class TemplateRepository : ITemplateRepository
    {
        readonly MongoContext mongoContext;
        const string TemplateCollection = "template";


        private IMongoCollection<Template> GetMongoCollection()
        {
            return mongoContext.GetCollection<Template>(TemplateCollection);
        }
        public TemplateRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }
        public Task Add(Template value)
        {
            return GetMongoCollection().InsertOneAsync(value);
        }

        public bool Remove(string id)
        {
            DeleteResult actionResult = GetMongoCollection().DeleteOne(p => p.Id == id);
            return actionResult.IsAcknowledged
                            && actionResult.DeletedCount > 0;
        }

        public bool Update(string id, Template valeu)
        {
            throw new NotImplementedException();
        }

        Task<List<Template>> IRepository<Template>.GetAll()
        {
            return GetMongoCollection().Find(_ => true).ToListAsync();
        }

        Task<List<Template>> IRepository<Template>.Get(Expression<Func<Template, bool>> expression)
        {
            return GetMongoCollection().Find(expression).ToListAsync();
        }
    }
}
