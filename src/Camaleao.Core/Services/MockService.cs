using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using Jint;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camaleao.Core.Services
{
    public class MockService : Notifiable, IMockService
    {
        private IEngineService _engine;
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
            var mapTemplateRequest = new Dictionary<string, object>();
            var mapRequest = new Dictionary<string, object>();

            MapperContract(_template.Request, mapTemplateRequest);
            MapperContract(_request, mapRequest);

            mapRequest.All(request =>
            {
                if (!mapTemplateRequest.ContainsKey(ClearNavigateProperties(request.Key)))
                {
                    AddNotification($"{request.Key}", "The propertie name don't reflect the contract");
                    return true;
                }

                if (mapTemplateRequest[ClearNavigateProperties(request.Key)].ToString().GetTypeChameleon() != _request.SelectToken(request.Key).GetTypeJson())
                    AddNotification($"{request.Key}", "The type of the propertie don't reflect the contract");
                return true;
            });

            return Notifications;
        }

        public IReadOnlyCollection<Notification> ValidateRules()
        {
            foreach(var rule in _template.Rules)
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
            var functions = ExtractList(expression, "##");

            functions.ForEach(func =>
            {
                var function = MapperFunction(ExtractBetween(func, "##").Split(','));

                if (execEngine)
                    expression = expression.Replace(function, _engine.Execute<string>(function));
                else
                    expression = expression.Replace(func, function);
            });

            return expression;
        }

        private string ExtractProperties(string expression, bool execEngine, string nameFunction = "GetElement", params string[] delimiters)
        {
            var properties = ExtractList(expression, delimiters);
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
            if(nameFunction == "GetComplexElement")
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

        private string ClearNavigateProperties(string key)
        {
            if (ExtractBetween(key, "[", "]") != String.Empty)
            {
                ExtractList(key, "[", "]").ForEach(k => key = key.Replace(k, "[0]"));
            }

            return key;
        }

        private static string ExtractBetween(string content, params string[] delimeters)
        {
            int StartPosition = 0, EndPosition = 0;

            if (content.Contains(delimeters.FirstOrDefault()) && content.Contains(delimeters.LastOrDefault()))
            {
                StartPosition = content.IndexOf(delimeters.FirstOrDefault(), EndPosition) + delimeters.FirstOrDefault().Length;
                EndPosition = content.IndexOf(delimeters.LastOrDefault(), StartPosition);
                return content.Substring(StartPosition, EndPosition - StartPosition);
            }

            return String.Empty;
        }

        private static string Extract(string content, params string[] delimeters)
        {
            string extracted = String.Empty;
            if ((extracted = ExtractBetween(content, delimeters)) != String.Empty)
                return String.Format(@"{0}{1}{2}", delimeters.FirstOrDefault(), extracted, delimeters.LastOrDefault());
            return extracted;
        }

        private static List<string> ExtractList(string content, params string[] delimeters)
        {
            var elements = new List<string>();

            string extracted = String.Empty;
            while ((extracted = Extract(content, delimeters)) != String.Empty)
            {
                content = content.Replace(extracted, String.Empty);
                elements.Add(extracted);
            }
            return elements;
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
                    return $"{parameters[0]}('{ExtractBetween(parameters[1], "{{", "}}")}', {parameters[2]})";
                case "GetElement":
                    return $"{parameters[0]}('{ExtractBetween(parameters[1], "{{", "}}")}')";
                case "GetComplexElement":
                    return $"{parameters[0]}('{ExtractBetween(parameters[1], "$$")}')";
            }

            return String.Empty;
        }

    }
}
