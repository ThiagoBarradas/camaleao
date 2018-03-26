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
        private Template _template;
        private JObject _request;
        private Response _response;

        public MockService(IEngineService engine)
        {
            _engine = engine;
        }

        public void InitializeMock(Template template, JObject request)
        {
            _template = template;
            _request = request;
            _engine.LoadRequest(request);
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

            return Notifications;
        }

        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach (var rule in _template.Rules)
            {
                var expression = rule.Expression;

                expression = ExtractFunctions(expression, false);
                expression = ExtractProperties(expression, false, delimiters: new string[] { "{{", "}}" });

                if (_engine.Execute<bool>(expression))
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
                var function = MapperFunction(func.ExtractBetween(func, "##").Split(','));

                if (execEngine)
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

                if (execEngine)
                    expression = expression.Replace(String.Format(StyleStringFormat(nameFunction), propertie), _engine.Execute<string>(function));
                else
                    expression = expression.Replace(String.Format(StyleStringFormat(nameFunction), propertie), function);
            });

            return expression;
        }

        private string StyleStringFormat(string nameFunction)
        {
            if (nameFunction == "GetComplexElement")
                return @"""{0}""";

            return @"{0}";
        }

        public Response Response()
        {
            _response = _template.Responses.FirstOrDefault(r => r.ResponseId == _response.ResponseId);
            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, delimiters: new string[] { "{{", "}}" });
            _response.Body = ExtractProperties(Convert.ToString(_response.Body), true, "GetComplexElement", delimiters: new string[] { "$$" });
            return _response;
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
                    return $"{parameters[0]}('{parameters[1].ExtractBetween("{{", "}}")}', {parameters[2]})";
                case "GetElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween("{{", "}}")}')";
                case "GetComplexElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween("$$")}')";
            }

            return String.Empty;
        }

    }
}
