using Camaleao.Core;
using Camaleao.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Repository
{
    public class TemplateRepository : ITemplateRepository
    {
        MongoContext mongoContext;
        const string TemplateCollection = "template";
        public TemplateRepository(Settings settings)
        {
            mongoContext = new MongoContext(settings);
        }
        public Task Add(Template value)
        {
            return mongoContext.GetCollection<Template>(TemplateCollection).InsertOneAsync(value);
        }

        public Task<Template> Get(Expression<Func<Template, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<Template> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remove(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(string id, Template valeu)
        {
            throw new NotImplementedException();
        }
    }
}
