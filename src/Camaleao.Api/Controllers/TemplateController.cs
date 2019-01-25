using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Application.TemplateAgg.Models.Exemplos;
using Camaleao.Application.TemplateAgg.Services;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Examples;
using System.Net;

namespace Camaleao.Api.Controllers {

    [Route(RouteConfig.Template)]
    [Produces("application/json")]
    public class TemplateController : Controller
    {
    
        private readonly IConfiguration _Configuration;
        private readonly ITemplateAppService _templateAppService;

        public TemplateController(  IConfiguration configuration,
                                    ITemplateAppService templateAppService)
        {

            _Configuration = configuration;
            _templateAppService = templateAppService;
        }

        /// <summary>
        /// Return Template 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="version"></param>
        /// <param name="routeName"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        [HttpGet("{user}/{version}/{routeName}/{method}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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
        [SwaggerRequestExample(typeof(CreateTemplateRequestModel), typeof(CreateTemplateSample))]
        [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(CreateTemplateResponseModelSample))]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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

        /// <summary>
        /// Create Response
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="responseModel"></param>
        /// <returns></returns>
        [HttpPost("{user}/createresponse")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult CreateResponse(string user, [FromBody]ResponseTemplateResquestModel responseModel) {

       
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

        /// <summary>
        /// Generate Template
        /// </summary>
        /// <param name="user"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("{user}/generate")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Generate(string user, [FromBody]dynamic body) {
            var response = _templateAppService.Generate(user, body);
            return new ObjectResult(response) { StatusCode = 200 };
        }

        /// <summary>
        /// Update Template
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="updateTemplateRequestModel"></param>
        /// <returns></returns>
        [HttpPut("{user}/{token}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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