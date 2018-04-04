using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Services
{
    public class MockService : Notifiable, IMockService
    {
        private readonly IEngineService _engine;
        private readonly IContextService _contextService;

        private Template _template;
        private JObject _request;
        private ResponseTemplate _response;
        private Context _context;

        public MockService(IEngineService engine, IContextService contextService)
        {
            _engine = engine;
            _contextService = contextService;
        }

        public void InitializeMock(Template template, JObject request)
        {
            _template = template;
            _request = request;
            _engine.LoadRequest(request, "_request");
        }

        public IReadOnlyCollection<Notification> ValidateContract()
        {
            var templateRequestMapped = new Dictionary<string, object>();
            var requestMapped = new Dictionary<string, object>();

            MapperContract(_template.Request, templateRequestMapped);
            MapperContract(_request, requestMapped);

            foreach (var request in requestMapped)
            {
                if (!templateRequestMapped.ContainsKey(request.Key.ClearNavigateProperties()))
                {
                    AddNotification($"{request.Key}", "The propertie name don't reflect the contract");
                    continue;
                }

                if (templateRequestMapped[request.Key.ClearNavigateProperties()].ToString().GetTypeChameleon() != _request.SelectToken(request.Key).GetTypeJson())
                    AddNotification($"{request.Key}", "The type of the propertie don't reflect the contract");
            }

            LoadContext(requestMapped, templateRequestMapped);

            return Notifications;
        }

        private string ExtractActionExpression(string expression)
        {
            expression = ExtractProperties(expression, false, "NoScope", "Context", delimiters: Delimiters.ContextVariable());
            expression = ExtractProperties(expression, false, "NoScope", "ContextComplex", delimiters: Delimiters.ContextComplexElement());
            expression = ExtractProperties(expression, true, "NoScope", delimiters: Delimiters.ElementRequest());
            expression = ExtractFunctions(expression, false);
            return expression;
        }

        private string ExtractRulesExpression(string expression)
        {
            expression = ExtractFunctions(expression, false);
            expression = ExtractProperties(expression, false, "NoScope", "Context", delimiters: Delimiters.ContextVariable());
            expression = ExtractProperties(expression, false, "NoScope", "ContextComplex", delimiters: Delimiters.ContextComplexElement());
            expression = ExtractProperties(expression, false, "NoScope", delimiters: Delimiters.ElementRequest());
            expression = ExtractProperties(expression, false, "NoScope", "GetComplexElement", delimiters: Delimiters.ComplexElement());

            return expression;
        }

        private string ExtractResponseExpression(string expression)
        {
            expression = ExtractProperties(expression, true, "Response", "Context", delimiters: Delimiters.ContextVariable());
            expression = ExtractProperties(expression, true, "Response", "GetContextComplexElement", delimiters: Delimiters.ContextComplexElement());
            expression = ExtractProperties(expression, true, "Response", delimiters: Delimiters.ElementRequest());
            expression = ExtractProperties(expression, true, "Response", "GetComplexElement", Delimiters.ComplexElement());

            return expression;
        }

        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach (var rule in _template.Rules)
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

        private string StyleStringFormat(string variableType, string scope, string nameFunction)
        {
            if (scope == "Response" && (variableType == "object" || variableType == "number" || nameFunction == "GetContextComplexElement"))
                return @"""{0}""";

            return @"{0}";
        }

        public ResponseTemplate Response()
        {

            _response = _template.Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);

            ExecuteActionTemplate();

            ExecuteActionResponse();

            _response.Body = ExtractResponseExpression(Convert.ToString(_response.Body));

            if (_context != null)
            {
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
            if (_template.Actions != null)
                _template.Actions.ForEach(action =>
                {
                    _engine.Execute<string>(ExtractActionExpression(action.Execute));
                });
        }

        private void LoadContext(Dictionary<string, object> requestMapped, Dictionary<string, object> templateRequestMapped)
        {
            string key = templateRequestMapped.FirstOrDefault(r => r.Value.ToString().Equals("_context")).Key ?? string.Empty;

            if (requestMapped.ContainsKey(key))
            {
                _context = _contextService.FirstOrDefault(requestMapped[key].ToString());
            }
            else if (_template.Context != null)
            {
                _context = _template.Context.CreateContext();
                _contextService.Add(_context);
            }
            _engine.Execute<string>(_context.GetVariablesAsString());

        }

        private void MapperContract(JToken request, Dictionary<string, dynamic> mapper)
        {

            var children = request.Children<JToken>();

            foreach (var item in children)
            {
                if (item.HasValues && item.First != null)
                {
                    MapperContract(item, mapper);
                }
                else
                {
                    mapper.Add(item.Path, item);
                }
            }
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

    }
}
