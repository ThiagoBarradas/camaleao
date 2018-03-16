using Camaleao.Core;
using Camaleao.Core.Services;
using Camaleao.Core.Services.Interfaces;
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

        private readonly ITemplateService _templateService;

        public MockController(MockService mockService, ITemplateService templateService)
        {
            _mockService = mockService;
            _templateService = templateService;
        }

        [HttpPost("{id}")]
        public IActionResult Post(string id, [FromBody]dynamic request)
        {
            var template = this._templateService.FirstOrDefault(p => p.Id == id);

            if (template == null)
                return NotFound("Identify Not Found");

            _mockService.ValidateContract(template, request);

            if(!_mockService.Valid)
                return new ObjectResult(_mockService.Notifications.FirstOrDefault().Message) { StatusCode = 400 };

            var rule = _mockService.ValidateRules(template.Rules);

            if(rule == null)
                return new ObjectResult("An error internal occurred") { StatusCode = 500 };

            Response response = _mockService.GetResponse(template, rule);

            return new ObjectResult(response.Body) { StatusCode = response.StatusCode };
        }

    }
}
