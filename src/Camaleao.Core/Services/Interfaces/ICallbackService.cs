using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface ICallbackService
    {
        void Add(Callback callback);
        Callback FirstOrDefault(Expression<Func<Callback, bool>> expression);
        List<Callback> Find(Expression<Func<Callback, bool>> expression);
        bool Update(string id, Callback value);
    }
}
