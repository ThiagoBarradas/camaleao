using Camaleao.Api.Controllers;
using Camaleao.Core.Services.Interfaces;
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
        private readonly IGetService _GetService;
        public RequestMappingMock(RequestDelegate next, IGetService getService)
        {
            this._next = next;
            this._GetService = getService;
        }


        public async Task Invoke(HttpContext context)
        {


            if (context.Request.Method != "GET")
            {
                await _next.Invoke(context);
                return;
            }
            else
            {
                await _GetService.Invoke(context, _next);
                return;
            }

        }
    }
}
