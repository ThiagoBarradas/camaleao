using Camaleao.Core;
using Camaleao.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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
            var rules = _mockService.ValidateContract(route, request);

            if(!_mockService.Valid)
                return new ObjectResult(_mockService.Notifications.FirstOrDefault().Message) { StatusCode = 400 };

            var rule = await _mockService.ValidateRule(rules);

            if (rule == null)
                return new ObjectResult("") { StatusCode = 500 };

            Response response = _mockService.GetResponse(rule);

            return new ObjectResult(response.Body) { StatusCode = response.StatusCode };
        }

    }
}
