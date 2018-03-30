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

            LoadContext(requestMapped, templateRequestMapped);
            
            return Notifications;
        }

        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach(var rule in _template.Rules)
            {
                var expression = rule.Expression;

                //if(rule.ResponseId.Equals("_callback") && _context == null)
                //    continue;

                expression = ExtractFunctions(expression, false);
                expression = ExtractProperties(expression, false, delimiters: new string[] { "{{", "}}" });
                expression = ExtractProperties(expression, false, "Context", delimiters: new string[] { "_context.{{", "}}" });

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
            //Context cback = null;
            //if(_response.ResponseId == "_callback")
            //{
            //    cback = _callbackService.FirstOrDefault(p => p.CID == _callback.CID);
            //    _response.ResponseId = cback.ResponseId;

            //    //_engine.LoadRequest(cback.Request, "_callbackRequest");

            //    _engine.Execute<string>(cback.Variables);
            //}

            _response = _template.Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);

            //tratar todas as execuções
            _response.Actions.ForEach(action =>
            {
                action.Execute = ExtractProperties(Convert.ToString(action.Execute), true, delimiters: new string[] { "{{", "}}" });
                action.Execute = ExtractProperties(Convert.ToString(action.Execute), true, "Context", delimiters: new string[] { "_context.{{", "}}" });

                _engine.Execute<string>(action.Execute);
            });


            //processar a expression
            //if(_response.Expression != null)
            //{
            //    if(cback == null)
            //    {
            //        _engine.Execute<string>(_response.Variables);
            //    }

            //    _response.Expression = ExtractProperties(Convert.ToString(_response.Expression), true, delimiters: new string[] { "{{", "}}" });
            //    _response.Expression = ExtractProperties(Convert.ToString(_response.Expression), false, "GetCallbackVar", delimiters: new string[] { "**" });

            //    _engine.Execute<string>(_response.Expression);

            //    var variaveis = _response.Variables.Trim().Split(';');

            //    for(int i = 0; i < variaveis.Length - 1; i++)
            //    {
            //        var name = variaveis[i].Split(' ')[1];
            //        var result = _engine.Execute<string>(name);
            //        variaveis[i] = $"var {name} = {result}";
            //    }

            //    _response.Variables = String.Join(";", variaveis);
            //}

            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, delimiters: new string[] { "{{", "}}" });
            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, "GetComplexElement", delimiters: new string[] { "$$" });
            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, "Context", delimiters: new string[] { "_context.{{", "}}" });

            _response.Body = Convert.ToString(_response.Body).Replace("_context", _context.Id);

            _context.Variables.ForEach(variable =>
            {
                var result = _engine.Execute<string>(variable.Name);
                variable.Value = result;
            });

            _contextService.Update(_context.Id.ToString(), _context);

            //if(Convert.ToString(_response.Body).Contains("_context"))
            //{
            //    _contextService.Add()
            //}


            //if(cback != null)
            //{
            //    cback.ResponseId = _response.Callback;
            //    cback.Variables = _response.Variables;
            //    _callbackService.Update(cback.CID, cback);
            //    _response.Body = Convert.ToString(_response.Body).Replace("_callback", cback.CID);
            //}
            //else if(_response.Callback != null)
            //{
            //    //LOAD CALLBACK
            //    var callback = new Callback() { CID = Guid.NewGuid().ToString(), ResponseId = _response.Callback, Variables =  _response.Variables };
            //    _callbackService.Add(callback);
            //    _response.Body = Convert.ToString(_response.Body).Replace("_callback", callback.CID);
            //}


            return _response;
        }

        private void LoadContext(Dictionary<string, object> requestMapped, Dictionary<string, object> templateRequestMapped)
        {
            string key = templateRequestMapped.FirstOrDefault(r => r.Value.ToString().Equals("_context")).Key ?? string.Empty;

            if(requestMapped.ContainsKey(key))
            {
                _context = _contextService.FirstOrDefault(p => p.Id.ToString() == requestMapped[key].ToString());
                var variaveis = MapperVariables(_context.Variables);
                _engine.Execute<string>(variaveis);
            }
        }

        private string MapperVariables(List<Variable> variables)
        {
            StringBuilder retorno = new StringBuilder();

            variables.ForEach(variable =>
            {
                retorno.Append($"var {variable.Name} = {variable.Initialize ?? "''"};");
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
