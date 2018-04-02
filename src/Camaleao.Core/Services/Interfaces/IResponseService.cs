using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IResponseService
    {
        void Add(ResponseTemplate response);
        void Add(IEnumerable<ResponseTemplate> responses);
        ResponseTemplate FirstOrDefault(Expression<Func<ResponseTemplate, bool>> expression);
        List<ResponseTemplate> Find(Expression<Func<ResponseTemplate, bool>> expression);
    }
}
