using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Application.TemplateAgg.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                    Version = path[2].ToLower()
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

        private static Task BadRequest<T>(HttpContext context, T obj) {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            _serializeJson<T>(obj, context.Response.Body);

            return Task.Factory.StartNew(() => context);
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

        private static T DeserializeJson<T>(Stream stream) {

            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr)) {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        private static void _serializeJson<T>(T obj, Stream stream) {

            using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            using (var jsonWriter = new JsonTextWriter(streamWriter)) {
                var serializer = new JsonSerializer();
                serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
                serializer.Formatting = Formatting.None;
                serializer.Serialize(jsonWriter, obj);
            }
        }
    }

}
