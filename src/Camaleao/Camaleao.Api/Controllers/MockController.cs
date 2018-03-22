using Camaleao.Core;
using Camaleao.Core.Services;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Camaleao.Api.Controllers
{
    [Route("api/mock")]
    public class MockController : Controller
    {
        private readonly IMockService _mockService;
        private readonly ITemplateService _templateService;

        public MockController(IMockService mockService, ITemplateService templateService)
        {
            _mockService = mockService;
            _templateService = templateService;
        }

        [HttpPost("{id}")]
        public IActionResult Post(string id, [FromBody]JObject request)
        {
            var template = this._templateService.FirstOrDefault(p => p.Id == id);

            if (template == null)
                return NotFound("Identify Not Found");

            _mockService.InitializeMock(template, request);
            var notifications = _mockService.ValidateContract();

            if(notifications.Any())
                return new ObjectResult(notifications) { StatusCode = 400 };

            return Ok();
            //var rule = _mockService.ValidateRules(template.Rules);

            //if(rule == null)
            //    return new ObjectResult("An error internal occurred") { StatusCode = 500 };

            //Response response = _mockService.GetResponse(template, rule);

            //return new ObjectResult(response.Body) { StatusCode = response.StatusCode };
        }

    }
}
