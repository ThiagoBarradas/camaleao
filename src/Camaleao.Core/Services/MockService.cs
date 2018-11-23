using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Core.Services {
    public class MockService : Notifiable, IMockService {


        private RequestMapped requestMapped;
        private readonly IContextService _contextService;
        private Context _context;
        private string _responseId;
        private PostbackTemplate _postbackTemplate;


        public MockService(IContextService contextService) {
            _contextService = contextService;

        }
        public void InitializeMock(RequestMapped request) {

            requestMapped = request;
            ((List<Notification>)this.Notifications).Clear();
        }

        public void LoadContext() {


            if (requestMapped.HasContext())
                _context = _contextService.FirstOrDefault(requestMapped.GetContext());
            else if (requestMapped.HasExternalContext()) {
                LoadContextByExternalIdentifier(requestMapped.GetExternalContext());
            }
            else
                CreateNewContext();

            if (_context != null)
                requestMapped.GetEngineService().Execute<string>(_context.GetVariablesAsString());

        }

        private void LoadContextByExternalIdentifier(string externalIdentifier) {

            _context = _contextService.FirstOrDefaultByExternalIdentifier(externalIdentifier);
            if (_context == null && externalIdentifier.IsGuid())
                CreateNewContext(externalIdentifier);
        }

        private void CreateNewContext(string externalIdentifier = "") {
            if (requestMapped.GetTemplate().Context != null) {
                _context = requestMapped.GetTemplate().Context.CreateContext();
                _context.ExternalIdentifier = externalIdentifier;
                _contextService.Add(_context);
            }
        }

        private void ExecuteActionTemplate() {
            if (requestMapped.GetTemplate().Actions != null)
                requestMapped.GetTemplate().Actions.ForEach(action => {
                    requestMapped.GetEngineService().Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private void ExecuteActionResponse(ResponseTemplate responseTemplate) {
            if (responseTemplate.Actions != null)
                responseTemplate.Actions.ForEach(action => {
                    requestMapped.GetEngineService().Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private string ExtractActionExpression(string expression) {

            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this.requestMapped.GetEngineService(),false, ScopeExpression.NoScope,true),
                new ExtractContextComplexElementExpression(this.requestMapped.GetEngineService(),false,ScopeExpression.NoScope,true),
                new ExtractElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.NoScope,true)
            }, expression);

        }

        private string ExtractResponseExpression(string expression) {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false),
                new ExtractContextComplexElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false),
                new ExtractElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false),
                new ExtractContextComplexElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false)
            }, expression);
        }



        private ResponseTemplate GetResponse(string responseId) {

            //  var response = requestMapped.GetTemplate().ResponsesId.FirstOrDefault(r => r.ResponseId == responseId);

            //var response = new ResponseTemplate();
            //ExecuteActionResponse(response);

            //response.Body = ExtractResponseExpression(Convert.ToString(response.Body));

            //if (_context != null) {
            //    response.Body = Convert.ToString(response.Body).Replace("_context.external", _context.ExternalIdentifier);
            //    response.Body = Convert.ToString(response.Body).Replace("_context", _context.Id.ToString());

            //    _context.Variables.ForEach(variable => {
            //        string value = "";

            //        if (variable.Type == "object" || variable.Type == "array")
            //            value = requestMapped.GetEngineService().Execute<string>($"JSON.stringify({variable.Name})");
            //        else
            //            value = requestMapped.GetEngineService().Execute<string>(variable.Name);

            //        if (variable.Type?.ToLower() == "text" && !string.IsNullOrEmpty(value))
            //            variable.Value = $"'{value}'";
            //        else if (variable.Type?.ToLower() == "bool" && value != null)
            //            variable.Value = value.ToLower();
            //        else
            //            variable.Value = value;

            //    });

            //    _contextService.Update(_context);
            //}

            return null;
        }
        public ResponseTemplate Response() {

            ExecuteActionTemplate();

            var response = this.GetResponse(this._responseId);

            if (_postbackTemplate != null) {
                Task.Run(() => { _postbackTemplate.Send(this.GetResponse(_postbackTemplate.ResponseId).Body, ExtractResponseExpression(_postbackTemplate.Url)); });
            }

            return response;
        }

        public IReadOnlyCollection<Notification> ValidateContract() {
            return requestMapped.ValidateContract();
        }

        public IReadOnlyCollection<Notification> ValidateRules() {

            this._postbackTemplate = null;

            foreach (var rule in requestMapped.GetTemplate().Rules) {
                if (string.IsNullOrWhiteSpace(rule.Expression) == false && requestMapped.GetEngineService().Execute<bool>(requestMapped.ExtractRulesExpression(rule.Expression))) {
                    this._responseId = rule.ResponseId;
                    if (rule.Postback != null)
                        this._postbackTemplate = rule.Postback;
                    return Notifications;
                }
            }

            AddNotification($"", "No rules match your request");
            return Notifications;
        }
    }
}
