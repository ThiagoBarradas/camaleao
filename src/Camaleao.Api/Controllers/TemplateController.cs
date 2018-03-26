using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Camaleao.Api.Controllers
{

    [Route("api/Template")]
    public class TemplateController : Controller
    {

        private readonly ITemplateService _templateService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;

        public TemplateController(ITemplateService templateService, IMapper mapper, IConfiguration configuration)
        {
            _templateService = templateService;
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