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

        public MockResponseModel Execute(MockRequestModel mockRequestModel) {
            var template = _templateRepository.Get(p => p.Route.Name.ToLower() == mockRequestModel.Name.ToLower()
                                 && p.User.ToLower() == mockRequestModel.User.ToLower()
                                 && p.Route.Method.ToLower() == mockRequestModel.Method.ToLower()
                                 && p.Route.Version.ToLower() == mockRequestModel.Version.ToLower()).FirstOrDefault();

            if (template == null)
                return new MockResponseModel(404, "Page Not Found!");

            RequestRecived requestRecived;

            if (mockRequestModel.Method.Equals("post"))
                requestRecived = Post(mockRequestModel, template);
            else
                requestRecived = Get(mockRequestModel, template);

            if (!requestRecived.IsValid())
                return new MockResponseModel(400, $"Invalid {mockRequestModel.Method}");

            Context context = GetContext(mockRequestModel.User, template, requestRecived);

            RuleTemplate ruleAnswered = GetRuleAnswered(template);

            if (ruleAnswered == null)
                return new MockResponseModel(400, "Rule Not Found");

            template.Actions?.ForEach(p => this._engineService.Execute<string>(p.Execute.ExtractExpressionAction(this._engineService)));

            var response = GetResponse(ruleAnswered.ResponseId, mockRequestModel.User, requestRecived, context);

            if (ruleAnswered.Postback != null)
                Task.Run(() => ruleAnswered.Postback.Send(GetResponse(ruleAnswered.Postback.ResponseId, mockRequestModel.User, requestRecived, context),
                                                                      ruleAnswered.Postback.Url.ExtractExpressionResponse(this._engineService)));

            return response;
        }

        private GetRequestRecived Get(MockRequestModel mockRequestModel, Template template) {

            var getRequestTemplate = (GetRequestTemplate)template.Request;
            return new GetRequestRecived(getRequestTemplate, mockRequestModel.QueryString);
        }



        private RequestRecived Post(MockRequestModel mockRequestModel, Template template) {
            var postRequestTemplate = (PostRequestTemplate)template.Request;
            var request = new PostRequestRecived(postRequestTemplate, mockRequestModel.HttpContext.Request.Body);
            this._engineService.LoadRequest(request.RequestRecived, "_request");
            return request;
        }

        private MockResponseModel GetResponse(string responseid, string user, RequestRecived requestRecived, Context context) {
            var response = _responseRepository.Get(p => p.ResponseId == responseid && p.User == user).FirstOrDefault();

            if (response == null)
                return new MockResponseModel(400, "Rule Not Found");

            response.Actions?.ForEach(p => this._engineService.Execute<string>(p.Execute.ExtractExpressionAction(this._engineService)));

            var bodyResponse = response.Body.ExtractExpressionResponse(this._engineService) ?? string.Empty;

            bodyResponse = bodyResponse.Replace(VariableTypeEnum.ExternalContext, requestRecived.GetContextIdentifier())
                          .Replace(VariableTypeEnum.Context, requestRecived.GetContextIdentifier());

            context.Variables.ForEach(variable => {

                
                if (variable.Type == VariableTypeEnum.objectType || variable.Type == VariableTypeEnum.Array)
                    variable.SetValue(this._engineService.Execute<string>($"JSON.stringify({variable.Name})", true));
                else if(variable.Type == VariableTypeEnum.Integer)
                    variable.SetValue(this._engineService.Execute<int>(variable.Name, true));
                else if(variable.Type == VariableTypeEnum.Boolean)
                    variable.SetValue(this._engineService.Execute<bool>(variable.Name, true));
                else
                    variable.SetValue(this._engineService.Execute<string>(variable.Name, true));
            });
            _contextRepository.Update(p => p.Id == context.Id, context);

            return new MockResponseModel(response.StatusCode, bodyResponse);
        }

        private Context GetContext(string user, Template template, RequestRecived requestRecived) {
            Context context = null;

            if (requestRecived.GetRequestTemplate().UseContext()) {
                Guid identifier = Guid.Parse(requestRecived.GetContextIdentifier());
                context = _contextRepository.Get(p => p.Id == identifier).FirstOrDefault();
            }
            else if (requestRecived.GetRequestTemplate().UseExternalContext()) {
                context = _contextRepository.Get(p => p.ExternalIdentifier == requestRecived.GetContextIdentifier() && p.User == user).FirstOrDefault();
            }

            if (context == null) {
                context = template.Context.CreateContext(requestRecived.GetContextIdentifier());
                _contextRepository.Add(context);
            }

            foreach (var variable in context.Variables) {
                _engineService.SetVariable(variable.Name, variable.Value, variable.Type);
            }
            return context;
        }

        private RuleTemplate GetRuleAnswered(Template template) {
            foreach (var rule in template.Rules.Where(p => !p.Expression.Equals(VariableTypeEnum.Default))) {
                if (this._engineService.Execute<bool>(rule.Expression.ExtractExpression(this._engineService))) {
                    return rule;
                }
            }
            return template.Rules.FirstOrDefault(p => p.Expression.Equals(VariableTypeEnum.Default));
        }
    }
}
