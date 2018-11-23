using AutoMapper;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Camaleao.Core.Repository;
using Camaleao.Core.Validates;

namespace Camaleao.Application.TemplateAgg.Services {
    public class TemplateAppService : ITemplateAppService {

        private readonly IMapper _mapper;
        private readonly ITemplateRepository _templateRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly ICreateTemplateValidate _createTemplateValidate;

        public TemplateAppService(IMapper mapper,
                                    ITemplateRepository templateRepository,
                                    IResponseRepository responseRepository,
                                    ICreateTemplateValidate createTemplateValidate) {
            this._mapper = mapper;
            this._templateRepository = templateRepository;
            this._responseRepository = responseRepository;
            this._createTemplateValidate = createTemplateValidate;
        }

        /// <summary>
        /// Método responsável por criar um novo Template
        /// </summary>
        /// <param name="user"></param>
        /// <param name="createTemplateRequestModel"></param>
        /// <returns></returns>
        public CreateTemplateResponseModel Create(string user, CreateTemplateRequestModel createTemplateRequestModel) {

            Template template = _mapper.Map<Template>(createTemplateRequestModel);
            template.AddUser(user);

            var responses = _mapper.Map<List<ResponseTemplate>>(createTemplateRequestModel.Responses);

            if (!template.IsValid())
                return new CreateTemplateResponseModel(400)
                    .AddErros(template.Notifications.Select(p => p.Message).ToList());

            if (!this._createTemplateValidate.Validate(template, responses))
                return new CreateTemplateResponseModel(400)
                   .AddErros(_createTemplateValidate.GetNotifications().Select(p => p.Message).ToList());

            var templateAux = _templateRepository.Get(p => p.User.ToLower() == user.ToLower() &&
                                           p.Route.Method.ToLower() == template.Route.Method.ToLower() &&
                                           p.Route.Name.ToLower() == template.Route.Name.ToLower() &&
                                           p.Route.Version.ToLower() == template.Route.Version.ToLower()).FirstOrDefault();

            if (templateAux != null)
                return new CreateTemplateResponseModel(400)
                    .AddError("This template already exists. Please update or create another version");
        
            
            template.AddResponses(responses);

            _templateRepository.Add(template);
            _responseRepository.Add(responses);
            return new CreateTemplateResponseModel(201) {
                Token = template.Id.ToString(),
                Route = $"api/{user}/{template.Route.Version}/{template.Route.Name}",
                Method = template.Route.Method
            };
        }

        /// <summary>
        /// Método responsável por obter um template
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetRequestTemplateResponseModel Get(GetRequestTemplateRequestModel request) {

            var template = _templateRepository.Get(p => p.Route.Name.ToLower() == request.RouteName.ToLower()
                                        && p.User.ToLower() == request.User.ToLower()
                                        && p.Route.Method.ToLower() == request.Method.ToLower()
                                        && p.Route.Version.ToLower() == request.Version.ToLower()).FirstOrDefault();

            if (template == null)
                return null;

            var responses = _responseRepository.Get(p => template.ResponsesId.Contains(p.Id));

            var getTemplateResponseModel = new GetRequestTemplateResponseModel();
            getTemplateResponseModel.Template = _mapper.Map<TemplateResponseModel>(template);
            var responsesModel = _mapper.Map<List<ResponseModel>>(responses);
            getTemplateResponseModel.Template.Responses = responsesModel;
            getTemplateResponseModel.Token = template.Id;
            return getTemplateResponseModel;
        }

        //public object Update(string user, UpdateTemplateRequestModel updateTemplateRequestModel) {

        //    if(! _templateRepository.Get(p => p.Id == updateTemplateRequestModel.Token).Any())
        //        return new CreateTemplateResponseModel(400)
        //            .AddError("Template does not exist to be updated.");

        //    Template template = _mapper.Map<Template>(updateTemplateRequestModel);
        //    template.AddUser(user);
        //    var responses = _mapper.Map<List<ResponseTemplate>>(updateTemplateRequestModel.Responses);


        //}
    }
}
