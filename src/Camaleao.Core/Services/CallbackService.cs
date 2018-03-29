using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Camaleao.Core.Services
{
    public class CallbackService : ICallbackService
    {
        readonly ICallbackRepository _callbackRepository;

        public CallbackService(ICallbackRepository callbackRepository)
        {
            _callbackRepository = callbackRepository;
        }
        public void Add(Callback callback)
        {
            _callbackRepository.Add(callback);
        }

        public bool Update(string id, Callback value)
        {
            return _callbackRepository.Update(id, value);
        }

        public List<Callback> Find(Expression<Func<Callback, bool>> expression)
        {
            return _callbackRepository.Get(expression).Result;
        }

        public Callback FirstOrDefault(Expression<Func<Callback, bool>> expression)
        {
            return _callbackRepository.Get(expression).Result.FirstOrDefault();
        }
    }
}
