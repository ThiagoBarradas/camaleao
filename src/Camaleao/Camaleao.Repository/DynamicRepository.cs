using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Repository
{
    public class DynamicRepository : IRepository<object>
    {
        MongoContext context;



        public DynamicRepository(Settings settings)
        {
            context = new MongoContext(settings);
        }

        public Task Add(object value)
        {
            return context.GetCollection<object>("configs").InsertOneAsync(value);
        }

        public Task<dynamic> Get(System.Linq.Expressions.Expression<Func<dynamic, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAll()
        {
            return null;
        }

        public Task<bool> Remove(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(string id, dynamic valeu)
        {
            throw new NotImplementedException();
        }
    }
}
