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
    public class MockService : Notifiable, IMockService
    {
        private readonly IEngineService _engine;
        private readonly IContextService _contextService;

        private Template _template;
        private JObject _request;
        private Response _response;
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

            foreach(var request in requestMapped)
            {
                if(!templateRequestMapped.ContainsKey(request.Key.ClearNavigateProperties()))
                {
                    AddNotification($"{request.Key}", "The propertie name don't reflect the contract");
                    continue;
                }

                if(templateRequestMapped[request.Key.ClearNavigateProperties()].ToString().GetTypeChameleon() != _request.SelectToken(request.Key).GetTypeJson())
                    AddNotification($"{request.Key}", "The type of the propertie don't reflect the contract");
            }

            if (_template.Context == null && requestMapped.Values.Contains("_context"))
                AddNotification($"Context", "There isn't mapped context in your template");
            else if(_template.Context != null)
                LoadContext(requestMapped, templateRequestMapped);

            return Notifications;
        }

        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach(var rule in _template.Rules)
            {
                var expression = rule.Expression;

                expression = ExtractFunctions(expression, false);
                expression = ExtractProperties(expression, false, "Context", delimiters: new string[] { "_context.{{", "}}" });
                expression = ExtractProperties(expression, false, delimiters: new string[] { "{{", "}}" });

                if(_engine.Execute<bool>(expression))
                {
                    _response = new Response() { ResponseId = rule.ResponseId };
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

                if(execEngine)
                    expression = expression.Replace(function, _engine.Execute<string>(function));
                else
                    expression = expression.Replace(func, function);
            });

            return expression;
        }

        private string ExtractProperties(string expression, bool execEngine, string nameFunction = "GetElement", params string[] delimiters)
        {
            var properties = expression.ExtractList(delimiters);
            properties.ForEach(propertie =>
            {
                var function = MapperFunction(nameFunction, propertie);

                if(execEngine)
                    expression = expression.Replace(String.Format(StyleStringFormat(nameFunction), propertie), _engine.Execute<dynamic>(function));
                else
                    expression = expression.Replace(String.Format(StyleStringFormat(nameFunction), propertie), function);
            });

            return expression;
        }

        private string StyleStringFormat(string nameFunction)
        {
            if(nameFunction == "GetComplexElement")
                return @"""{0}""";

            return @"{0}";
        }

        public Response Response()
        {
            _response = _template.Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);

            _response.Actions.ForEach(action =>
            {
                action.Execute = ExtractProperties(Convert.ToString(action.Execute), false, "Context", delimiters: new string[] { "_context.{{", "}}" });
                action.Execute = ExtractProperties(Convert.ToString(action.Execute), true, delimiters: new string[] { "{{", "}}" });

                _engine.Execute<string>(action.Execute);
            });


            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, "Context", delimiters: new string[] { "_context.{{", "}}" });
            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, delimiters: new string[] { "{{", "}}" });
            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, "GetComplexElement", delimiters: new string[] { "$$" });
            

            if(_context != null)
            {
                _response.Body = Convert.ToString(_response.Body).Replace("_context", _context.Id.ToString());

                _context.Variables.ForEach(variable =>
                {
                    var result = _engine.Execute<string>(variable.Name);
                    variable.Value = result;
                });

                _contextService.Update(_context.Id.ToString(), _context);
            }
            

            return _response;
        }

        private void LoadContext(Dictionary<string, object> requestMapped, Dictionary<string, object> templateRequestMapped)
        {
            string key = templateRequestMapped.FirstOrDefault(r => r.Value.ToString().Equals("_context")).Key ?? string.Empty;

            if(requestMapped.ContainsKey(key))
            {
                _context = _contextService.FirstOrDefault(p => p.Id == Guid.Parse(requestMapped[key].ToString()));

                if(_context == null)
                {
                    AddNotification($"Context", $"There isn't registered context with this ID: {requestMapped[key]}");
                    return;
                }
            }
            else
            {
                _context = _template.Context;
                _context.TemplateId = _template.Id;
                _contextService.Add(_context);
            }

            var variaveis = MapperVariables(_context.Variables);
            _engine.Execute<string>(variaveis);
        }

        private string MapperVariables(List<Variable> variables)
        {
            StringBuilder retorno = new StringBuilder();

            variables.ForEach(variable =>
            {
                retorno.Append($"var {variable.Name} = {variable.Value};");
            });

            return retorno.ToString();
        }

        private void MapperContract(JToken request, Dictionary<string, dynamic> mapper)
        {

            var children = request.Children<JToken>();

            foreach(var item in children)
            {
                if(item.HasValues && item.First != null)
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
            switch(parameters.FirstOrDefault())
            {
                case "Contains":
                case "NotContains":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween("{{", "}}")}', {parameters[2]})";
                case "ExistPath":
                case "NotExistPath":
                case "GetElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween("{{", "}}")}')";
                case "GetComplexElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween("$$")}')";
                case "Context":
                    return $"{parameters[1].ExtractBetween("_context.{{", "}}")}";
            }

            return String.Empty;
        }

    }
}
