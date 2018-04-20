using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camaleao.Core.Services
{
    public class MockService :Notifiable, IMockService
    {


        private RequestMapped requestMapped;
        private readonly IContextService _contextService;
        private Context _context;
        private ResponseTemplate _response;


        public MockService(IContextService contextService)
        {
            _contextService = contextService;

        }
        public void InitializeMock(RequestMapped request)
        {

            requestMapped = request;
            ((List<Notification>)this.Notifications).Clear();
        }

        public void LoadContext()
        {


            if(requestMapped.HasContext())
                _context = _contextService.FirstOrDefault(requestMapped.GetContext());
            else if(requestMapped.HasExternalContext())
            {
                LoadContextByExternalIdentifier(requestMapped.GetExternalContext());
            }
            else
                CreateNewContext();

            if(_context != null)
                requestMapped.GetEngineService().Execute<string>(_context.GetVariablesAsString());

        }

        private void LoadContextByExternalIdentifier(string externalIdentifier)
        {

            _context = _contextService.FirstOrDefaultByExternalIdentifier(externalIdentifier);
            if(_context == null && externalIdentifier.IsGuid())
                CreateNewContext(externalIdentifier);
        }

        private void CreateNewContext(string externalIdentifier = "")
        {
            if(requestMapped.GetTemplate().Context != null)
            {
                _context = requestMapped.GetTemplate().Context.CreateContext();
                _context.ExternalIdentifier = externalIdentifier;
                _contextService.Add(_context);
            }
        }

        private void ExecuteActionTemplate()
        {
            if(requestMapped.GetTemplate().Actions != null)
                requestMapped.GetTemplate().Actions.ForEach(action =>
                {
                    requestMapped.GetEngineService().Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private void ExecuteActionResponse()
        {
            if(_response.Actions != null)
                _response.Actions.ForEach(action =>
                {
                    requestMapped.GetEngineService().Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private string ExtractActionExpression(string expression)
        {

            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this.requestMapped.GetEngineService(),false, ScopeExpression.NoScope,true),
                new ExtractContextComplexElementExpression(this.requestMapped.GetEngineService(),false,ScopeExpression.NoScope,true),
                new ExtractElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.NoScope,true)
            }, expression);

        }

        private string ExtractResponseExpression(string expression)
        {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false),
                new ExtractContextComplexElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false),
                new ExtractElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false),
                new ExtractContextComplexElementExpression(this.requestMapped.GetEngineService(),true,ScopeExpression.Response,false)
            }, expression);
        }

        public ResponseTemplate Response()
        {
            _response = requestMapped.GetTemplate().Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);

            ExecuteActionTemplate();

            ExecuteActionResponse();

            _response.Body = ExtractResponseExpression(Convert.ToString(_response.Body));

            if(_context != null)
            {
                _response.Body = Convert.ToString(_response.Body).Replace("_context.external", _context.ExternalIdentifier);
                _response.Body = Convert.ToString(_response.Body).Replace("_context", _context.Id.ToString());

                _context.Variables.ForEach(variable =>
                {
                    string value = "";

                    if(variable.Type == "object" || variable.Type == "array")
                        value = requestMapped.GetEngineService().Execute<string>($"JSON.stringify({variable.Name})");
                    else
                        value = requestMapped.GetEngineService().Execute<string>(variable.Name);

                    if(variable.Type?.ToLower() == "text" && !string.IsNullOrEmpty(value))
                        variable.Value = $"'{value}'";
                    else
                        variable.Value = value;

                });

                _contextService.Update(_context);
            }

            return _response;
        }

        public IReadOnlyCollection<Notification> ValidateContract()
        {
            return requestMapped.ValidateContract();
        }

        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach(var rule in requestMapped.GetTemplate().Rules)
            {
                if(requestMapped.GetEngineService().Execute<bool>(requestMapped.ExtractRulesExpression(rule.Expression)))
                {
                    _response = new ResponseTemplate() { ResponseId = rule.ResponseId };
                    return Notifications;
                }
            }

            AddNotification($"", "No rules match your request");
            return Notifications;
        }
    }
}
