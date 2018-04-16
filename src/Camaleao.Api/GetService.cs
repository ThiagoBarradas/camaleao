using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Camaleao.Api
{
    public class GetService :IGetService
    {

        private readonly IGetMockService _mockService;
        private readonly ITemplateService _templateService;
        private readonly IResponseService _responseService;

        public GetService(IGetMockService mockService, ITemplateService templateService, IResponseService responseService)
        {
            _mockService = mockService;
            _templateService = templateService;
            _responseService = responseService;
        }
        public Task Invoke(HttpContext context, RequestDelegate next)
        {
            string[] path = context.Request.Path.Value.Split("/");

            if(path.Length < 4 || path[2].ToLower().Equals("template"))
                return next.Invoke(context);

            string user = path[2];
            string version = path[3];

            string[] queryString = path.Skip(5).Take(path.Length - 5).ToArray();

            var template = _templateService.FirstOrDefault(p => p.User == user && p.Route.Version == version && p.Route.Method == "GET");

            if(template == null)
            {
                return BadRequest(context, "Identify Not Found");
            }


            template.Responses = _responseService.Find(p => p.TemplateId == template.Id);
            _mockService.StartUp(template, queryString);

            var notifications = _mockService.ValidateContract();
            if(notifications.Any())
                return BadRequest(context, notifications);

            _mockService.LoadContext();

            notifications = _mockService.ValidateRules();
            if(notifications.Any())
                return BadRequest(context, notifications);

            var response = _mockService.Response();

            return OK(context, response.Body, response.StatusCode);

        }

        private static Task BadRequest<T>(HttpContext context, T obj)
        {
            context.Response.StatusCode = 400;

            _serializeJson<T>(obj, context.Response.Body);

            return Task.Factory.StartNew(() => context);
        }

        private static Task OK<T>(HttpContext context, T obj, int statusCode)
        {
            context.Response.StatusCode = statusCode;

            context.Response.ContentType = "application/json";
            context.Response.WriteAsync(obj.ToString());

            return Task.Factory.StartNew(() => context);
        }

        private static void _serializeJson<T>(T obj, Stream stream)
        {

            using(var streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            using(var jsonWriter = new JsonTextWriter(streamWriter))
            {
                var serializer = new JsonSerializer();
                serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
                serializer.Formatting = Formatting.None;
                serializer.Serialize(jsonWriter, obj);
            }
        }


    }

}
