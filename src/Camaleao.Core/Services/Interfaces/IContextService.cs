using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IContextService
    {
        void Add(Context callback);
        Context FirstOrDefault(Expression<Func<Context, bool>> expression);
        List<Context> Find(Expression<Func<Context, bool>> expression);
        bool Update(string id, Context value);
    }
}
