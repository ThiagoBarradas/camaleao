﻿using AutoMapper;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Camaleao.Core.Repository;
using Camaleao.Core.Validates;
using MongoDB.Bson;

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

        public object CreateResponse(string user, ResponseModel responseModel) {

            var response = _mapper.Map<ResponseTemplate>(responseModel);
            response.AddUser(user);

            if (!response.IsValid())
                return new CreateTemplateResponseModel(400)
                    .AddErros(response.Notifications.Select(p => p.Message).ToList());

            return null;
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
                    .AddError("This template already exists for this user. Please update or create another version");


            foreach (var response in responses) {
                var responseExist = _responseRepository.Get(p => p.ResponseId == response.ResponseId && p.User == user).FirstOrDefault();
                if (responseExist != null)
                    return new CreateTemplateResponseModel(400)
                    .AddError($"This response[{response.ResponseId}] already exists for this user. Please update or create another version");
            }
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
        /// Método responsável por gerar um template apartir de um request
        /// </summary>
        /// <param name="user"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public GetRequestTemplateResponseModel Generate(string user, dynamic body) {
            Template template = new Template(user);
            PostRequestTemplate request = PostRequestTemplate.CreatePostRequestFromPost(body);
            template.Request = request;
            template.Route = new RouteTemplate("v1", "route", "post");

            _templateRepository.Add(template);

            var getTemplateResponseModel = new GetRequestTemplateResponseModel();
            getTemplateResponseModel.Template = _mapper.Map<TemplateResponseModel>(template);
            getTemplateResponseModel.Token = template.Id.ToString();
            getTemplateResponseModel.Template.Responses = new List<ResponseModel>();
            return getTemplateResponseModel;
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
            getTemplateResponseModel.Token = template.Id.ToString();
            return getTemplateResponseModel;
        }

        public CreateTemplateResponseModel Update(string user, UpdateTemplateRequestModel updateTemplateRequestModel) {

            if (!_templateRepository.Get(p => p.Id == ObjectId.Parse(updateTemplateRequestModel.Token)).Any())
                return new CreateTemplateResponseModel(400)
                    .AddError("Template does not exist to be updated.");

            Template template = _mapper.Map<Template>(updateTemplateRequestModel);
            template.AddUser(user);


            var responses = _mapper.Map<List<ResponseTemplate>>(updateTemplateRequestModel.Responses);

            foreach (var response in responses) {

                var responseAux = _responseRepository.Get(p => p.ResponseId == response.ResponseId && p.User == user).FirstOrDefault();
                if (responseAux != null) {
                    response.UpdateId(responseAux.Id);
                    _responseRepository.Update(p => p.Id == response.Id, response);
                }
                else {
                    _responseRepository.Add(response);
                }
            }
            template.AddResponses(responses);
            _templateRepository.Update(p => p.Id == template.Id, template);

            return new CreateTemplateResponseModel(200) {
                Token = template.Id.ToString(),
                Route = $"api/{user}/{template.Route.Version}/{template.Route.Name}",
                Method = template.Route.Method
            };
        }
    }
}
