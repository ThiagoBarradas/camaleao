using Camaleao.Api.Controllers;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camaleao.Api.Middlewares {
    public class RequestMappingMock {

        private readonly RequestDelegate _next;
        private readonly IMockApiService mockApiService;

        public RequestMappingMock(RequestDelegate next, IMockApiService mockApiService) {
            this._next = next;
            this.mockApiService = mockApiService;

        }

        public async Task Invoke(HttpContext context) {

            if ((context.Request.Method == "GET" || context.Request.Method == "POST")
                && !RouteConfig.PathContainsRoutes(context.Request.Path)) {
                await mockApiService.Invoke(context, _next);
            }
            else {
                await _next.Invoke(context);

            }
        }
    }
}
