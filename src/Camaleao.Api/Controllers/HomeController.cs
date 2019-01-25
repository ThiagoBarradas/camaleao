using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace Camaleao.Api.Controllers
{
    [Route("/")]
    [Produces("application/json")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Home API Description
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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
