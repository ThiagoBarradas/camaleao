using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Camaleao.Api.Controllers
{

    [Route("api/Template")]
    public class TemplateController : Controller
    {


        ITemplateService _templateService;
        IMapper _mapper;

        public TemplateController(ITemplateService templateService, IMapper mapper)
        {
            _templateService = templateService;
            _mapper = mapper;
        }

        // POST api/values
        [HttpPost]
        public IActionResult Create([FromBody]TemplateRequest templateRequest)
        {

            if (ModelState.IsValid)
            {
                var template = _mapper.Map<Template>(templateRequest);
                _templateService.Add(template);
            }
         
            return Ok();
        }
    }
}