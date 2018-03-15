using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Camaleao.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Camaleao.Api.Controllers
{
    [Route("api/mock")]
    public class MockController : Controller
    {
        private readonly MockService _mockService;

        public MockController(MockService mockService)
        {
            _mockService = mockService;
        }

        [HttpPost("{route}")]
        public async Task<IActionResult> Post(string route, [FromBody]dynamic request)
        {
            _mockService.ValidateContract(route, request);

            if(!_mockService.Valid)
                return new ObjectResult(_mockService.Notifications.FirstOrDefault().Message) { StatusCode = 400 };


            return null;
        }

    }
}
