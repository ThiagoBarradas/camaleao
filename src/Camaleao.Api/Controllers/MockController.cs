using Camaleao.Core;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Camaleao.Api.Controllers
{
    [Route("api/mock")]
    public class MockController : Controller
    {
        private readonly IMockService _mockService;
        private readonly ITemplateService _templateService;
        private readonly IResponseService _responseService;

        public MockController(IMockService mockService, ITemplateService templateService, IResponseService responseService)
        {
            _mockService = mockService;
            _templateService = templateService;
            _responseService = responseService;
        }

        [HttpPost("{id}")]
        public IActionResult Post(string id, [FromBody]JObject request)
        {
            var template = _templateService.FirstOrDefault(p => p.Id == id);

            if (template == null)
                return NotFound("Identify Not Found");

            template.Responses = _responseService.Find(p => p.TemplateId == template.Id);
            _mockService.InitializeMock(template, request);

            var notifications = _mockService.ValidateContract();
            if(notifications.Any())
                return new ObjectResult(notifications) { StatusCode = 400 };

            notifications = _mockService.ValidateRules();
            if (notifications.Any())
                return new ObjectResult(notifications) { StatusCode = 400 };

            var response = _mockService.Response();

            return new ObjectResult(response.Body) { StatusCode = response.StatusCode };
        }

    }
}
