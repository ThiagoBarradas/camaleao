using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Api.Controllers
{

    [Route("api/template")]
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly IResponseService _responseService;
        private readonly IContextService _contextService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;

        public TemplateController(ITemplateService templateService, IResponseService responseService, IContextService contextService, IMapper mapper, IConfiguration configuration)
        {
            _templateService = templateService;
            _responseService = responseService;
            _contextService = contextService;
            _mapper = mapper;
            _Configuration = configuration;
        }

        [HttpGet("{user}/{version}/{routeName}")]
        public IActionResult Get(string user, string version, string routeName)
        {
            var template = _templateService.FirstOrDefault(p => p.User == user && p.Route.Name == routeName && p.Route.Version == version);

            if(template == null)
                return NotFound("Identify Not Found");

            template.Responses = _responseService.Find(p => p.TemplateId == template.Id);

            return new ObjectResult(JsonConvert.SerializeObject(template)) { StatusCode = 200 };
        }

        [HttpPost("{user}")]
        public IActionResult Create(string user, [FromBody]TemplateRequestModel templateRequest)
        {
            if (ModelState.IsValid)
            {
                var template = _mapper.Map<Template>(templateRequest);

                var notifications = _templateService.ValidateTemplate(template);
                if (notifications.Any())
                    return new ObjectResult(notifications) { StatusCode = 400 };

                template.Context?.Variables.ForEach(variable => variable.BuildVariable());
                template.User = user;
                _templateService.Add(template);

                template.Responses.ForEach(resp => resp.TemplateId = template.Id);
                _responseService.Add(template.Responses);


                TemplateResponseModelOk templateResponse = new TemplateResponseModelOk()
                {
                    Token = template.Id,
                    Route = $"{_Configuration["Host:Url"]}api/{user}/{template.Route.Version}/{template.Route.Name}"
                };
                return Ok(templateResponse);
            }
            else
                return BadRequest(ModelState.GetErrorResponse());
        }
    }
}