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
    public class MockService2 :Notifiable, IMockService
    {

        private Template template;
        private RequestMapped requestMapped;
        private readonly IContextService _contextService;
        private readonly IEngineService _engine;
        private Context _context;
        private ResponseTemplate _response;
        public void InitializeMock(Template template, RequestMapped request)
        {
            this.template = template;
            requestMapped = request;
        }

        public void LoadContext()
        {

            string externalKey = template.Request.GetMappedExternalContextKey();

            if(requestMapped.HasContext(template.Request.GetMappedContextKey()))
                _context = _contextService.FirstOrDefault(requestMapped.GetContext());
            else if(requestMapped.HasExternalContext(externalKey))
            {
                LoadContextByExternalIdentifier(requestMapped.GetExternalContext());
            }
            else
                CreateNewContext();

            if(_context != null)
                _engine.Execute<string>(_context.GetVariablesAsString());

        }

        private void LoadContextByExternalIdentifier(string externalIdentifier)
        {

            _context = _contextService.FirstOrDefaultByExternalIdentifier(externalIdentifier);
            if(_context == null && externalIdentifier.IsGuid())
                CreateNewContext(externalIdentifier);
        }

        private void CreateNewContext(string externalIdentifier = "")
        {
            if(template.Context != null)
            {
                _context = template.Context.CreateContext();
                _context.ExternalIdentifier = externalIdentifier;
                _contextService.Add(_context);
            }
        }

        private void ExecuteActionTemplate()
        {
            if(template.Actions != null)
                template.Actions.ForEach(action =>
                {
                    _engine.Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private void ExecuteActionResponse()
        {
            if(_response.Actions != null)
                _response.Actions.ForEach(action =>
                {
                    _engine.Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private string ExtractActionExpression(string expression)
        {

            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this._engine),
                new ExtractContextComplexElementExpression(this._engine),
                new ExtractElementExpression(this._engine)
            }, expression);

        }

        private string ExtractResponseExpression(string expression)
        {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this._engine),
                new ExtractContextComplexElementExpression(this._engine),
                new ExtractElementExpression(this._engine),
                new ExtractContextComplexElementExpression(this._engine)
            }, expression);
        }

        public ResponseTemplate Response()
        {
            _response = template.Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);

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
                        value = _engine.Execute<string>($"JSON.stringify({variable.Name})");
                    else
                        value = _engine.Execute<string>(variable.Name);

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
            foreach(var rule in template.Rules)
            {
                if(_engine.Execute<bool>(requestMapped.ExtractRulesExpression(rule.Expression)))
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
