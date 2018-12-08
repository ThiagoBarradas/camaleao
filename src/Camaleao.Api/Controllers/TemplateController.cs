using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Application.TemplateAgg.Models.Exemplos;
using Camaleao.Application.TemplateAgg.Services;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Examples;

namespace Camaleao.Api.Controllers {

    [Route(RouteConfig.Template)]
    public class TemplateController : Controller
    {

        private readonly IResponseService _responseService;
        private readonly IConfiguration _Configuration;
        private readonly ITemplateAppService _templateAppService;

        public TemplateController(IResponseService responseService,
                                    IConfiguration configuration,
                                    ITemplateAppService templateAppService)
        {

            _responseService = responseService;
            _Configuration = configuration;
            _templateAppService = templateAppService;
        }

        [HttpGet("{user}/{version}/{routeName}/{method}")]
        public IActionResult Get(string user, string version, string routeName, string method)
        {

            var response = _templateAppService.Get(new GetRequestTemplateRequestModel()
            {
                Method = method,
                RouteName = routeName,
                User = user,
                Version = version
            });
           
            return new ObjectResult(response) { StatusCode = 200 };
        }

        /// <summary>
        /// Create Template
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="templateModel"></param>
        /// <response code="201">Template created</response>
        [HttpPost("{user}")]
        [Consumes("application/json")]
        [SwaggerRequestExample(typeof(CreateTemplateRequestModel), typeof(CreateTemplateExemple))]
        public IActionResult Create(string user, [FromBody]CreateTemplateRequestModel templateModel)
        {
            if (ModelState.IsValid)
            {
                var response = _templateAppService.Create(user, templateModel);

                if (response.StatusCode == 201)
                    return new CreatedResult("create", response);
                else
                   return BadRequest(response);          
            }
            else
                return BadRequest(ModelState.GetErrorResponse());
        }

        [HttpPost("{user}/createresponse")]
        public IActionResult CreateResponse(string user, [FromBody]CreateResponseTemplateResquestModel responseModel) {

       
            if (ModelState.IsValid) {
                var response = _templateAppService.CreateResponse(user, responseModel);

                if (response.StatusCode == 201)
                    return new CreatedResult("create", response.ResponseModel);
                else
                    return BadRequest(response);
            }
            else
                return BadRequest(ModelState.GetErrorResponse());
        }

        [HttpPost("{user}/generate")]
        public IActionResult Generate(string user, [FromBody]dynamic body) {
            var response = _templateAppService.Generate(user, body);
            return new ObjectResult(response) { StatusCode = 200 };
        }

        [HttpPut("{user}/{token}")]
        public IActionResult Update(string user, string token, [FromBody]UpdateTemplateRequestModel updateTemplateRequestModel)
        {
            if (ModelState.IsValid) {
                var response = _templateAppService.Update(user, updateTemplateRequestModel);

                if (response.StatusCode == 200)
                    return new CreatedResult("updated", response);
                else
                    return BadRequest(response);
            }
            else
                return BadRequest(ModelState.GetErrorResponse());

        }
    }
}