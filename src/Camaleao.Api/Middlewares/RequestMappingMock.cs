using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camaleao.Api.Middlewares
{
    public class RequestMappingMock
    {

        private readonly RequestDelegate _next;
        public RequestMappingMock(RequestDelegate next)
        {
            this._next = next;
        }


        public async Task Invoke(HttpContext context)
        {


            if (context.Request.Method != "GET")
            {
                await _next.Invoke(context);
                return;
            }
          
        }
    }
}
