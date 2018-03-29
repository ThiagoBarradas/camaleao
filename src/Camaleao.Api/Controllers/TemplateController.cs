using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Camaleao.Api.Controllers
{

    [Route("api/Template")]
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

        // POST api/values
        [HttpPost]
        public IActionResult Create([FromBody]TemplateRequest templateRequest)
        {
            if (ModelState.IsValid)
            {
                var template = _mapper.Map<Template>(templateRequest);
                _templateService.Add(template);
                template.Responses.ForEach(resp => resp.TemplateId = template.Id);

                _responseService.Add(template.Responses);


                TemplateResponse templateResponse = new TemplateResponse()
                {
                    Token = template.Id,
                    Route = _Configuration["Host:Url"] +"api/mock/"+template.Id
                };
                return Ok(templateResponse);
            }
            else
                return BadRequest(ModelState.GetErrorResponse());
        }
    }
}