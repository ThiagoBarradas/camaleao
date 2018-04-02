using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
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
        public void Add(ResponseTemplate response)
        {
            _responseRepository.Add(response);
        }

        public void Add(IEnumerable<ResponseTemplate> responses)
        {
            _responseRepository.Add(responses);
        }

        public List<ResponseTemplate> Find(Expression<Func<ResponseTemplate, bool>> expression)
        {
            return _responseRepository.Get(expression).Result;
        }

        public ResponseTemplate FirstOrDefault(Expression<Func<ResponseTemplate, bool>> expression)
        {
            return _responseRepository.Get(expression).Result.FirstOrDefault();
        }
    }
}
