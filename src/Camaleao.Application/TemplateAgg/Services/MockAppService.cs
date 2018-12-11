using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using Camaleao.Core.Entities.Request;
using Camaleao.Core.Enuns;
using Camaleao.Core.Repository;
using Camaleao.Core.SeedWork;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Camaleao.Application.TemplateAgg.Services {
    public class MockAppService : IMockAppService {

        private readonly ITemplateRepository _templateRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IContextRepository _contextRepository;
        private readonly IEngineService _engineService;

        public MockAppService(ITemplateRepository templateRepository,
                              IResponseRepository responseRepository,
                              IContextRepository contextRepository,
                              IEngineService engineService) {

            this._templateRepository = templateRepository;
            this._responseRepository = responseRepository;
            this._contextRepository = contextRepository;
            this._engineService = engineService;
        }

        public MockResponseModel Get(MockRequestModel mockRequestModel) {

            var template = _templateRepository.Get(p => p.Route.Name.ToLower() == mockRequestModel.Name.ToLower()
                                       && p.User.ToLower() == mockRequestModel.User.ToLower()
                                       && p.Route.Method.ToLower() == mockRequestModel.Method.ToLower()
                                       && p.Route.Version.ToLower() == mockRequestModel.Version.ToLower()).FirstOrDefault();

            if (template == null)
                return new MockResponseModel(404, "Page Not Found!");

            return new MockResponseModel(404, "Page Not Found!");

        }

        public MockResponseModel Execute(MockRequestModel mockRequestModel) {
            if (mockRequestModel.Method.Equals("post"))
                return Post(mockRequestModel);
            else
                return Get(mockRequestModel);
        }

        private MockResponseModel Post(MockRequestModel mockRequestModel) {
            // buscar o template
            var template = _templateRepository.Get(p => p.User == mockRequestModel.User &&
                    p.Route.Version == mockRequestModel.Version &&
                    p.Route.Name == mockRequestModel.Name &&
                     p.Route.Method == mockRequestModel.Method).FirstOrDefault();


            //Comparar se o Request recebido satifaz as informações do template
            var postRequestTemplate = (PostRequestTemplate)template.Request;
            var postRequestRecived = new PostRequestRecived(postRequestTemplate, mockRequestModel.HttpContext.Request.Body);

            //Se o request não for válido eu retorno um badrequest
            if (!postRequestRecived.IsValid())
                return new MockResponseModel(400, "Request Inválid");

            // Carrego a requisição no engine
            this._engineService.LoadRequest(postRequestRecived.RequestRecived, "_request");

            //Pesquiso o contexto
            Context context = GetContext(mockRequestModel.User, template, postRequestTemplate, postRequestRecived);

            //Percorro as regras para achar o response
            RuleTemplate ruleAnswered = GetRuleAnswered(template);

            if (ruleAnswered == null)
                return new MockResponseModel(400, "Request Inválid");

            //Executo as actions globais
            template.Actions?.ForEach(p => this._engineService.Execute<string>(p.Execute.ExtractExpressionAction(this._engineService)));

            //Executo as actions do response
            var response = GetResponse(ruleAnswered.ResponseId, mockRequestModel.User, postRequestRecived, context);

            if (ruleAnswered.Postback != null)
                Task.Run(() => ruleAnswered.Postback.Send(GetResponse(ruleAnswered.Postback.ResponseId, mockRequestModel.User, postRequestRecived, context),
                                                                      ruleAnswered.Postback.Url.ExtractExpressionResponse(this._engineService)));

            return response;
        }

        private MockResponseModel GetResponse(string responseid, string user, PostRequestRecived postRequestRecived, Context context) {
            var response = _responseRepository.Get(p => p.ResponseId == responseid && p.User == user).First();
            response.Actions?.ForEach(p => this._engineService.Execute<string>(p.Execute.ExtractExpressionAction(this._engineService)));

            var bodyResponse = response.Body.ExtractExpressionResponse(this._engineService) ?? string.Empty;

            bodyResponse = bodyResponse.Replace(VariableTypeEnum.ExternalContext, postRequestRecived.GetContextIdentifier())
                          .Replace(VariableTypeEnum.Context, postRequestRecived.GetContextIdentifier());

            context.Variables.ForEach(variable => {
                if (variable.Type == VariableTypeEnum.objectType || variable.Type == VariableTypeEnum.Array)
                    variable.SetValue(this._engineService.Execute<string>($"JSON.stringify({variable.Name})"));
                else
                    variable.SetValue(this._engineService.Execute<string>(variable.Name));
            });
            _contextRepository.Update(p => p.Id == context.Id, context);

            return new MockResponseModel(response.StatusCode, bodyResponse);
        }

        private Context GetContext(string user, Template template, PostRequestTemplate postRequestTemplate, PostRequestRecived postRequestRecived) {
            Context context = null;

            if (postRequestTemplate.UseContext()) {
                Guid identifier = Guid.Parse(postRequestRecived.GetContextIdentifier());
                context = _contextRepository.Get(p => p.Id == identifier).FirstOrDefault();
            }
            else if (postRequestTemplate.UseExternalContext()) {
                context = _contextRepository.Get(p => p.ExternalIdentifier == postRequestRecived.GetContextIdentifier() && p.User == user).FirstOrDefault();
            }

            if (context == null) {
                context = template.Context.CreateContext(postRequestRecived.GetContextIdentifier());
                _contextRepository.Add(context);
            }

            return context;
        }

        private RuleTemplate GetRuleAnswered(Template template) {
            foreach (var rule in template.Rules) {
                if (this._engineService.Execute<bool>(rule.Expression.ExtractExpression(this._engineService))) {
                    return rule;
                }
            }
            return template.Rules.FirstOrDefault(p => p.Expression.Equals(VariableTypeEnum.Default));
        }
    }
}
