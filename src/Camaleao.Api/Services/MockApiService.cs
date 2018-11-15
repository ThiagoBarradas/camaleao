using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Camaleao.Core;
using Camaleao.Core.Entities;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Context;

namespace Camaleao.Api {
    public class MockApiService : IMockApiService {

        private readonly IMockService _mockService;
        private readonly ITemplateService _templateService;
        private readonly IResponseService _responseService;
        private readonly IEngineService _engineService;

        RequestMapped requestMapped = null;
        public MockApiService(IMockService mockService, ITemplateService templateService, IResponseService responseService, IEngineService engineService) {
            _mockService = mockService;
            _templateService = templateService;
            _responseService = responseService;
            _engineService = engineService;

        }

        public Task Invoke(HttpContext context, RequestDelegate next) {
            Guid requestKey = Guid.NewGuid();
            try {
                string[] path = context.Request.Path.Value.Split("/").Skip(1).ToArray();

                string user = path[1].ToLower();

                var route = RouteTemplate.Create(path[3], path[2], context.Request.Method);

                if (!route.IsValid())
                    return BadRequest(context, "Route Invalid");

                var template = _templateService.FindByRoute(user, route);

                if (template == null)
                    return BadRequest(context, "Route Not Found");

               // template.AddResponses(_responseService.Find(p => p.Id == template.Id));

                if (template.Route.Method.ToUpper() == "GET") {
                    string[] queryString = path.Skip(4).ToArray();
                    requestMapped = new GetRequestMapped(template, this._engineService, queryString);
                }
                else if (template.Route.Method.ToUpper() == "POST") {

                    var body = DeserializeJson<JObject>(context.Request.Body);
                    string bodyText = body.ToString();
                    using (LogContext.PushProperty("Content", bodyText))
                    using (LogContext.PushProperty("RequestKey", requestKey)) {
                        Log.Information("Request recived");
                    }
                    requestMapped = new PostRequestMapped(template, this._engineService, body);
                }

                _mockService.InitializeMock(requestMapped);

                var notifications = _mockService.ValidateContract();
                if (notifications.Any())
                    return BadRequest(context, notifications);

                _mockService.LoadContext();

                notifications = _mockService.ValidateRules();
                if (notifications.Any())
                    return BadRequest(context, notifications);

                var response = _mockService.Response();

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
