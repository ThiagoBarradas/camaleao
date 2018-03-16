using Camaleao.Core.ExtensionMethod;
using Flunt.Notifications;
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
    public class MockService : Notifiable
    {
        private Dictionary<string, object> _requestTemplate;
        private Dictionary<string, object> _requestContent;

        public MockService()
        {
            _requestTemplate = new Dictionary<string, object>();
            _requestContent = new Dictionary<string, object>();
        }

        public void ValidateContract(Template template, dynamic request)
        {
            MapperContract(template.Request, _requestTemplate);
            MapperContract(request, _requestContent);

            var contentTyped = _requestContent.ConvertType(_requestTemplate);

            if(!_requestTemplate.All(k => _requestContent.ContainsKey(k.Key) 
                                                && contentTyped[k.Key].GetType() == k.Value.ToString().GetTypeChameleon()
                                                && ValidateArrayContract(k.Key, contentTyped[k.Key], _requestTemplate)))
                AddNotification("BadRequest", "The request don't reflect the contract");

            _requestContent = contentTyped;
        }

        public Rule ValidateRules(IList<Rule> rules)
        {
            foreach(var rule in rules)
            {
                var expression = rule.Expression;
                expression = expression.Replace(_requestContent, true);

                var result = Processador(expression).Result;
                if(result)
                    return rule;
            }
            return null;
        }

        public Response GetResponse(Template template, Rule rule)
        {
            var response = template.Responses.FirstOrDefault(p => p.Id == rule.ResponseId).Response;
            response.Body = response.Body_.Replace(_requestContent, false);
            return response;
        }

        private bool ValidateArrayContract(string key, object arrayRequest, Dictionary<string, object> requestTemplate)
        {
            if(arrayRequest.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
            {
                Dictionary<string, object> input = new Dictionary<string, object>();
                Dictionary<string, object> template = new Dictionary<string, object>();
                MapperContract(JToken.Parse(arrayRequest.ToString()), input);
                MapperContract(JToken.Parse(requestTemplate[key].ToString()), template);

                var contentTyped = input.ConvertType(template);

                if(!template.All(tpl => ValidateArrayContract(tpl.Key, contentTyped[tpl.Key], template) 
                                                && input.All(ipt => ipt.Key.Substring(4) == tpl.Key.Substring(4))))
                    return false;
            }

            return true;
        }

        private void MapperContract(JToken input, Dictionary<string, object> receive)
        {
            var children = input.Children<JToken>();

            foreach(var item in children)
            {
                if(item.GetType() == typeof(JArray))
                {
                    receive.Add(item.Path, item);
                    break;
                }
                else if(item.HasValues && item.First != null)
                {
                    MapperContract(item, receive);
                }
                else
                {
                    receive.Add(item.Path, item.ToString());
                }
            }
        }

        private async Task<bool> Processador(string content)
        {
            object result = await CSharpScript.EvaluateAsync(content);
            return Convert.ToBoolean(result);
        }
    }
}
