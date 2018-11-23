using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Application.TemplateAgg.Services;
using Camaleao.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Camaleao.Api.Controllers {

    [Route(RouteConfig.Template)]
    public class TemplateController : Controller
    {

        private readonly IResponseService _responseService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _Configuration;
        private readonly ITemplateAppService _templateAppService;

        public TemplateController(IResponseService responseService,
                                    IMapper mapper,
                                    IConfiguration configuration,
                                    ITemplateAppService templateAppService)
        {

            _responseService = responseService;
            _mapper = mapper;
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

        [HttpPost("{user}")]
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

        //[HttpPut("{user}/{token}")]
        //public IActionResult Update(string user, string token, [FromBody]TemplateRequestModel templateRequest)
        //{

        //    if (ModelState.IsValid)
        //    {

        //        var templateOld = _templateService.Find(p => p.Id == token).FirstOrDefault();

        //        if (templateOld != null)
        //        {
        //            templateRequest.Id = templateOld.Id;

        //            var templateNew = _mapper.Map<Template>(templateRequest);

        //            _responseService.RemoveByTemplateId(templateOld.Id);

        //            if (!templateNew.IsValid())
        //                return new ObjectResult(templateNew.Notifications) { StatusCode = 400 };

        //            templateNew.User = user;

        //            _responseService.Add(templateNew.ResponsesId);
        //            _templateService.Update(templateNew);

        //            TemplateResponseModelOk templateResponse = new TemplateResponseModelOk()
        //            {
        //                Token = templateNew.Id,
        //                Route = $"{_Configuration["Host:Url"]}api/{user}/{templateNew.Route.Version}/{templateNew.Route.Name}"
        //            };
        //            return Ok(templateResponse);
        //        }
        //        else
        //            return BadRequest("Template Not Exist!");
        //    }
        //    else
        //        return BadRequest(ModelState.GetErrorResponse());
        //}
    }
}