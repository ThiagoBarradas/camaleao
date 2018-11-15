using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Camaleao.Api.Controllers
{
    [Route(RouteConfig.Context)]
    public class ContextController : Controller
    {
        private readonly IContextService _contextService;
        private readonly IMapper _mapper;

        public ContextController(IContextService contextService, IMapper mapper)
        {
            _contextService = contextService;
            _mapper = mapper;
        }

        [HttpGet("{contextId}")]
        public IActionResult Post(string contextId)
        {
            //var context = _mapper.Map<ContextModel>(_contextService.FirstOrDefault(contextId));

            //var notifications = _contextService.Notifications();
            //if(notifications.Any())
            //    return new ObjectResult(notifications) { StatusCode = 400 };

            //var response = JsonConvert.SerializeObject(context);
            //response = response.Replace(JsonConvert.SerializeObject(context), context.BuildVariables());

            //return new ObjectResult(response) { StatusCode = 200 };

            return Ok();
        }

    }
}
