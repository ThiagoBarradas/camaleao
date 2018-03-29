using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Camaleao.Core.Services
{
    public class ResponseService : IResponseService
    {
        readonly IResponseRepository _responseRepository;

        public ResponseService(IResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
        }
        public void Add(Response response)
        {
            _responseRepository.Add(response);
        }

        public void Add(IEnumerable<Response> responses)
        {
            _responseRepository.Add(responses);
        }

        public List<Response> Find(Expression<Func<Response, bool>> expression)
        {
            return _responseRepository.Get(expression).Result;
        }

        public Response FirstOrDefault(Expression<Func<Response, bool>> expression)
        {
            return _responseRepository.Get(expression).Result.FirstOrDefault();
        }
    }
}
