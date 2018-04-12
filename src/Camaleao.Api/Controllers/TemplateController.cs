using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Api.Controllers
{

    [Route("api/template")]
    public class TemplateController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly IResponseService _responseService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;

        public TemplateController(ITemplateService templateService, IResponseService responseService, IMapper mapper, IConfiguration configuration)
        {
            _templateService = templateService;
            _responseService = responseService;
            _mapper = mapper;
            _Configuration = configuration;
        }

        [HttpGet("{user}/{version}/{routeName}")]
        public IActionResult Get(string user, string version, string routeName)
        {
            var template = _templateService.FirstOrDefault(p => p.User == user && p.Route.Name == routeName && p.Route.Version == version);

            if (template == null)
                return NotFound("Identify Not Found");

            template.Responses = _responseService.Find(p => p.TemplateId == template.Id);

            var templateResponse = _mapper.Map<TemplateResponseModel>(template);

            var response = JsonConvert.SerializeObject(templateResponse);
            response = response.Replace(JsonConvert.SerializeObject(templateResponse.Context.Variables), templateResponse.Context.BuildVariables());

            return new ObjectResult(response) { StatusCode = 200 };
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

        [HttpPut("{user}/{token}")]
        public IActionResult Update(string user,string token, [FromBody]TemplateRequestModel templateRequest)
        {

            if (ModelState.IsValid)
            {
              
                var templeateOld = _templateService.FirstOrDefault(p => p.Id==token);

                if (templeateOld != null)
                {
                    var templateNew = _mapper.Map<Template>(templateRequest);

                    _responseService.RemoveByTemplateId(templeateOld.Id);

                    templateNew.Id = templeateOld.Id;

                     var notifications = _templateService.ValidateTemplate(templateNew);
                    if (notifications.Any())
                        return new ObjectResult(notifications) { StatusCode = 400 };

                    templateNew.Context?.Variables.ForEach(variable => variable.BuildVariable());
                    templateNew.User = user;


                    templateNew.Responses.ForEach(resp => resp.TemplateId = templateNew.Id);
                    _responseService.Add(templateNew.Responses);

                    _templateService.Update(templateNew);

                    TemplateResponseModelOk templateResponse = new TemplateResponseModelOk()
                    {
                        Token = templateNew.Id,
                        Route = $"{_Configuration["Host:Url"]}api/{user}/{templateNew.Route.Version}/{templateNew.Route.Name}"
                    };
                    return Ok(templateResponse);
                }
                else
                    return BadRequest("Template Not Exist!");
            }
            else
                return BadRequest(ModelState.GetErrorResponse());
        }
    }
}