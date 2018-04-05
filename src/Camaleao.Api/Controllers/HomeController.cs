using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace Camaleao.Api.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var content = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Documents/index.html");
            
            return new ContentResult()
            {
                StatusCode = 200,
                ContentType = "text/plain",
                Content = content
            };
        }
    }
}
