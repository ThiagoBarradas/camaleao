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
        private Template _template;
        private JObject _request;

        public void InitializeMock(Template template, JObject request)
        {
            _template = template;
            _request = request;
        }

        public IReadOnlyCollection<Notification> ValidateContract()
        {
            var mapTemplateRequest = new Dictionary<string, object>();
            var mapRequest = new Dictionary<string, object>();

            MapperContract(_template.Request, mapTemplateRequest);
            MapperContract(_request, mapRequest);

            if(!mapTemplateRequest.All(requestTemplate => mapRequest.ContainsKey(requestTemplate.Key))
                    && mapRequest.All(request => mapTemplateRequest[ClearNavigateProperties(request.Key)].ToString().GetTypeChameleon() != _request.SelectToken(request.Key).GetTypeJson()))
                AddNotification($"", "The request don't reflect the contract");

            return Notifications;
        }

        //public Rule ValidateRules(IList<Rule> rules)
        //{
        //    foreach(var rule in rules)
        //    {
        //        var expression = rule.Expression;
        //        expression = expression.Replace(_requestContent, true);
                
        //        var result = Processador(expression);
        //        if(result)
        //            return rule;
        //    }
        //    return null;
        //}

        //public Response GetResponse(Template template, Rule rule)
        //{
        //    var response = template.Responses.FirstOrDefault(p => p.Id == rule.ResponseId).Response;
        //    response.Body = response.Body_.Replace(_requestContent, false);
        //    return response;
        //}

        private string ClearNavigateProperties(string key)
        {
            if(ExtractBetween(key, "[", "]") != String.Empty)
            {
                ExtractList(key, "[", "]").ForEach(k => key = key.Replace(k, "[0]"));
            }

            return key;
        }

        private static string ExtractBetween(string content, params string[] delimeters)
        {
            int StartPosition = 0, EndPosition = 0;

            if(content.Contains(delimeters.FirstOrDefault()) && content.Contains(delimeters.LastOrDefault()))
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
            if((extracted = ExtractBetween(content, delimeters)) != String.Empty)
                return String.Format(@"{0}{1}{2}", delimeters.FirstOrDefault(), extracted, delimeters.LastOrDefault());
            return extracted;
        }

        private static List<string> ExtractList(string content, params string[] delimeters)
        {
            var elements = new List<string>();

            string extracted = String.Empty;
            while((extracted = Extract(content, delimeters)) != String.Empty)
            {
                content = content.Replace(extracted, String.Empty);
                elements.Add(extracted);
            }
            return elements;
        }

        //private bool ValidateArrayContract(string key, object arrayRequest, Dictionary<string, object> requestTemplate)
        //{
        //    if(arrayRequest.GetType() == typeof(JArray))
        //    {
        //        Dictionary<string, object> input = new Dictionary<string, object>();
        //        Dictionary<string, object> template = new Dictionary<string, object>();
        //        MapperContract(JToken.Parse(arrayRequest.ToString()), input);
        //        MapperContract(JToken.Parse(requestTemplate[key].ToString()), template);

        //        var contentTyped = input.ConvertType(template);

        //        if(!template.All(tpl => ValidateArrayContract(tpl.Key, contentTyped[tpl.Key], template) 
        //                                        && input.All(ipt => ipt.Key.Substring(4) == tpl.Key.Substring(4))))
        //            return false;
        //    }

        //    return true;
        //}

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

        //private bool Processador(string content)
        //{
        //    //object result = await CSharpScript.EvaluateAsync(content);
        //    //return Convert.ToBoolean(result);
        //    var engine = new Engine();
        //    var all = File.ReadAllText(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Pock"), "script.js"));
        //    engine.Execute(all);
        //    return Convert.ToBoolean(engine.Execute(content).GetCompletionValue().ToString());

        //}
    }
}
