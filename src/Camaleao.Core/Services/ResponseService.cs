using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Camaleao.Core.Entities;

namespace Camaleao.Core.Services
{
    public class ResponseService : IResponseService
    {
        readonly IResponseRepository _responseRepository;

        public ResponseService(IResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
        }
        public void Add(ResponseTemplate response) => 
            _responseRepository.Add(response);
     
        public void Add(IEnumerable<ResponseTemplate> responses) =>
            _responseRepository.Add(responses);

        public List<ResponseTemplate> Find(Expression<Func<ResponseTemplate, bool>> expression) =>
            _responseRepository.Get(expression).Result;

        public ResponseTemplate FirstOrDefault(Expression<Func<ResponseTemplate, bool>> expression) =>
             _responseRepository.Get(expression).Result.FirstOrDefault();

        public void Remove(List<ResponseTemplate> responses) =>
            responses.ForEach(p => _responseRepository.Remove(r => r.Id == p.Id));

        public void RemoveByTemplateId(string templateId) =>
            _responseRepository.Remove(p => p.TemplateId == templateId);
            
    
    }
}
