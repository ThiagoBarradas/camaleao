using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IResponseService
    {
        void Add(ResponseTemplate response);
        void Add(IEnumerable<ResponseTemplate> responses);
        ResponseTemplate FirstOrDefault(Expression<Func<ResponseTemplate, bool>> expression);
        List<ResponseTemplate> Find(Expression<Func<ResponseTemplate, bool>> expression);
        void RemoveByTemplateId(string templateId);
    }
}
