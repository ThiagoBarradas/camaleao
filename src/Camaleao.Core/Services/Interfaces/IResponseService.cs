using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IResponseService
    {
        void Add(Response response);
        void Add(IEnumerable<Response> responses);
        Response FirstOrDefault(Expression<Func<Response, bool>> expression);
        List<Response> Find(Expression<Func<Response, bool>> expression);
    }
}
