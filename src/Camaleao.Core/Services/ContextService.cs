using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Camaleao.Core.Services
{
    public class ContextService : IContextService
    {
        readonly IContextRepository _callbackRepository;

        public ContextService(IContextRepository callbackRepository)
        {
            _callbackRepository = callbackRepository;
        }
        public void Add(Context callback)
        {
            _callbackRepository.Add(callback);
        }

        public bool Update(string id, Context value)
        {
            return _callbackRepository.Update(id, value);
        }

        public List<Context> Find(Expression<Func<Context, bool>> expression)
        {
            return _callbackRepository.Get(expression).Result;
        }

        public Context FirstOrDefault(Expression<Func<Context, bool>> expression)
        {
            return _callbackRepository.Get(expression).Result.FirstOrDefault();
        }
    }
}
