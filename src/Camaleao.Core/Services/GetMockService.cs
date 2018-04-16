using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Mappers;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camaleao.Core.Services
{
    public class GetMockService : Notifiable, IGetMockService
    {

        private Template template;
        private ResponseTemplate _response;
        private readonly IEngineService _engine;
        private readonly IContextService _contextService;
        private Dictionary<int, string> _QueryStringMapped;
        private Context _context;

        public GetMockService(IEngineService engine, IContextService contextService)
        {
            _engine = engine;
            _contextService = contextService;
        }
        public void StartUp(Template template, string[] queryString)
        {
            this.template = template;
            _QueryStringMapped = MapQueryString(queryString);
        }
        private Dictionary<int, string> MapQueryString(string[] queryString)
        {
            Dictionary<int, string> identifier = new Dictionary<int, string>();

            for (int i = 0; i < queryString.Length; i++)
            {
                identifier.Add(i, queryString[i]);
            }

            return identifier;
        }

        public IReadOnlyCollection<Notification> ValidateContract()
        {

            foreach (var item in template.Request.GetIdentifierFromQueryString())
            {

                if (!this._QueryStringMapped.ContainsKey(item.Key))
                {
                    AddNotification($"{item.Key}", "The propertie name don't reflect the contract");
                    continue;
                }
            }
            return Notifications;
        }
        private string ExtractRulesExpression(string expression)
        {
            string newExpression = expression;

            this.template.Request.GetIdentifierFromQueryString()
                .Where(p => p.Value != "_context" && p.Value != "_context.external").ForEach(x =>
                {
                    if (expression.Contains(x.Value))
                        newExpression = expression.Replace(x.Value, this._QueryStringMapped[x.Key]);
                });

            newExpression = ExtractFunctions(newExpression, false);
            newExpression = ExtractProperties(newExpression, false, "NoScope", "Context", delimiters: Delimiters.ContextVariable());
            newExpression = ExtractProperties(newExpression, false, "NoScope", "ContextComplex", delimiters: Delimiters.ContextComplexElement());
            return newExpression;
        }
        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach (var rule in template.Rules)
            {
                if (_engine.Execute<bool>(ExtractRulesExpression(rule.Expression)))
                {
                    _response = new ResponseTemplate() { ResponseId = rule.ResponseId };
                    return Notifications;
                }
            }

            AddNotification($"", "No rules match your request");
            return Notifications;
        }

        private string ExtractFunctions(string expression, bool execEngine)
        {
            var functions = expression.ExtractList("##");

            functions.ForEach(func =>
            {
                var function = MapperFunction(func.ExtractBetween("##").Split(','));

                if (execEngine)
                    expression = expression.Replace(function, _engine.Execute<string>(function));
                else
                    expression = expression.Replace(func, function);
            });

            return expression;
        }

        private string ExtractProperties(string expression, bool execEngine, string scope, string nameFunction = "GetElement", params string[] delimiters)
        {
            var properties = expression.ExtractList(delimiters);
            properties.ForEach(propertie =>
            {
                var content = MapperFunction(nameFunction, propertie);

                if (execEngine)
                    expression = expression.Replace(String.Format(StyleStringFormat(_engine.VariableType(content), scope, nameFunction), propertie), _engine.Execute<dynamic>(content));
                else
                    expression = expression.Replace(String.Format(StyleStringFormat(_engine.VariableType(content), scope, nameFunction), propertie), content);
            });

            return expression;
        }

        private string MapperFunction(params string[] parameters)
        {
            switch (parameters.FirstOrDefault())
            {
                case "Contains":
                case "NotContains":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween(Delimiters.ElementRequest())}', {parameters[2]})";
                case "ExistPath":
                case "NotExistPath":
                case "GetElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween(Delimiters.ElementRequest())}')";
                case "Context":
                    return $"{parameters[1].ExtractBetween(Delimiters.ContextVariable())}";
                case "ContextComplex":
                    return $"{parameters[1].ExtractBetween(Delimiters.ContextComplexElement())}";
                case "GetComplexElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween(Delimiters.ComplexElement())}')";
                case "GetContextComplexElement":
                    return $"JSON.stringify({parameters[1].ExtractBetween(Delimiters.ContextComplexElement())})";
                default:
                    return $"{parameters[0]}({string.Join(',', parameters.Skip(1).ToArray())})";
            }
        }

        private string StyleStringFormat(string variableType, string scope, string nameFunction)
        {
            if (scope == "Response" && (variableType == "object" || variableType == "number" || nameFunction == "GetContextComplexElement"))
                return @"""{0}""";

            return @"{0}";
        }

        public void LoadContext()
        {
            var key = this.template.Request.GetIdentifierFromQueryString().FirstOrDefault(p => p.Value.Equals("_context"));
            var externalKey = this.template.Request.GetIdentifierFromQueryString().FirstOrDefault(p => p.Value.Equals("_context.external"));

            if (!key.Equals(default(KeyValuePair<int, string>)) && _QueryStringMapped.ContainsKey(key.Key))
                _context = _contextService.FirstOrDefault(_QueryStringMapped[key.Key].ToString());
            else if (!externalKey.Equals(default(KeyValuePair<int, string>)) && _QueryStringMapped.ContainsKey(externalKey.Key))
            {
                LoadContextByExternalIdentifier(externalKey.Key);
            }
            else
                CreateNewContext();

            if (_context != null)
                _engine.Execute<string>(_context.GetVariablesAsString());

        }
        private void LoadContextByExternalIdentifier(int externalKey)
        {
            string externalIdentifier = _QueryStringMapped[externalKey].ToString();
            _context = _contextService.FirstOrDefaultByExternalIdentifier(externalIdentifier);
            if (_context == null && externalIdentifier.IsGuid())
                CreateNewContext(externalIdentifier);
        }


        private void CreateNewContext(string externalIdentifier = "")
        {
            if (this.template.Context != null)
            {
                _context = template.Context.CreateContext();
                _context.ExternalIdentifier = externalIdentifier;
                _contextService.Add(_context);
            }
        }
        private string ExtractResponseExpression(string expression)
        {
            expression = ExtractProperties(expression, true, "Response", "Context", delimiters: Delimiters.ContextVariable());
            expression = ExtractProperties(expression, true, "Response", "GetContextComplexElement", delimiters: Delimiters.ContextComplexElement());
            expression = ExtractProperties(expression, true, "Response", delimiters: Delimiters.ElementRequest());
            expression = ExtractProperties(expression, true, "Response", "GetComplexElement", Delimiters.ComplexElement());

            return expression;
        }
        private string ExtractActionExpression(string expression)
        {
            expression = ExtractProperties(expression, false, "NoScope", "Context", delimiters: Delimiters.ContextVariable());
            expression = ExtractProperties(expression, false, "NoScope", "ContextComplex", delimiters: Delimiters.ContextComplexElement());
            expression = ExtractProperties(expression, true, "NoScope", delimiters: Delimiters.ElementRequest());
            expression = ExtractFunctions(expression, false);
            return expression;
        }
        private void ExecuteActionResponse()
        {
            if (_response.Actions != null)
                _response.Actions.ForEach(action =>
                {
                    _engine.Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private void ExecuteActionTemplate()
        {
            if (template.Actions != null)
                template.Actions.ForEach(action =>
                {
                    _engine.Execute<string>(ExtractActionExpression(action.Execute));
                });
        }
        public ResponseTemplate Response()
        {
            _response = template.Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);

            ExecuteActionTemplate();

            ExecuteActionResponse();

            _response.Body = ExtractResponseExpression(Convert.ToString(_response.Body));

            if (_context != null)
            {
                _response.Body = Convert.ToString(_response.Body).Replace("_context.external", _context.ExternalIdentifier);
                _response.Body = Convert.ToString(_response.Body).Replace("_context", _context.Id.ToString());

                _context.Variables.ForEach(variable =>
                {
                    string value = "";

                    if (variable.Type == "object" || variable.Type == "array")
                        value = _engine.Execute<string>($"JSON.stringify({variable.Name})");
                    else
                        value = _engine.Execute<string>(variable.Name);

                    if (variable.Type.ToLower() == "text" && !string.IsNullOrEmpty(value))
                        variable.Value = $"'{value}'";
                    else
                        variable.Value = value;

                });

                _contextService.Update(_context);
            }

            return _response;
        }
    }
}
