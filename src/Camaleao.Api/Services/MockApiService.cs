using System;
using System.Linq;
using System.Threading.Tasks;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Application.TemplateAgg.Services;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;

namespace Camaleao.Api {
    public class MockApiService : IMockApiService {

        private readonly IMockAppService mockAppService;


        public MockApiService(IMockAppService mockAppService) {

            this.mockAppService = mockAppService;
        }

        public Task Invoke(HttpContext context, RequestDelegate next) {
            Guid requestKey = Guid.NewGuid();
            try {
                string[] path = context.Request.Path.Value.Split("/").Skip(1).ToArray();


                var mockRequestModel = new MockRequestModel() {
                    HttpContext = context,
                    Method = context.Request.Method.ToLower(),
                    User = path[1].ToLower(),
                    Name = path[3].ToLower(),
                    Version = path[2].ToLower(),
                    QueryString= path.Skip(4).ToArray()
                };

                MockResponseModel response = mockAppService.Execute(mockRequestModel);

                return OK(context, response.Body, response.StatusCode);
            }
            catch (Exception ex) {
                using (LogContext.PushProperty("RequestKey", requestKey)) {
                    Log.Error(ex, "Error in Invoke");
                }

                return InternalServerError(context, ex.Message);
            }
        }


        private static Task OK<T>(HttpContext context, T obj, int statusCode) {
            context.Response.StatusCode = statusCode;

            context.Response.ContentType = "application/json";
            context.Response.WriteAsync(obj.ToString());

            return Task.Factory.StartNew(() => context);
        }

        private static Task InternalServerError<T>(HttpContext context, T obj) {
            return OK(context, obj, 500);
        }
    }
}
