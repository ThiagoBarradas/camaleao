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
        private JArray _responsesConf;

        public MockService()
        {
            _requestTemplate = new Dictionary<string, object>();
            _requestContent = new Dictionary<string, object>();
        }

        public JArray ValidateContract(Template template, dynamic request)
        {

            #region BUSCAR NO BANCO DE DADOS 
            JObject camaleao = null;
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Pock");
            using(StreamReader sr = new StreamReader(Path.Combine(path, route, "camaleao.json")))
            {
                camaleao = JObject.Parse(sr.ReadToEnd());
            }

            var requestInput = JObject.Parse(Convert.ToString(request));

            var requestConf = JObject.Parse(Convert.ToString(camaleao.SelectToken("Request")));
            _responsesConf = (JArray)camaleao.SelectToken("Responses");
            #endregion

            MapperContract(requestConf, _requestTemplate);
            MapperContract(requestInput, _requestContent);

            var contentTyped = _requestContent.ConvertType(_requestTemplate);

            if(!_requestTemplate.All(k => _requestContent.ContainsKey(k.Key) && contentTyped[k.Key].GetType() == k.Value.ToString().GetTypeChameleon()))
                AddNotification("BadRequest", "The request don't reflect the contract");

            _requestContent = contentTyped;

            return (JArray)camaleao.SelectToken("Rules");
        }

        public async Task<JToken> ValidateRule(JArray rules)
        {
            foreach(var rule in rules)
            {
                var expression = rule.SelectToken("Expression").ToString();
                expression = expression.Replace(_requestContent, true);

                var result = await Processador(expression);
                if(result)
                    return rule;
                    //var response = responses.First(p => Convert.ToString(p.SelectToken("Name")) == Convert.ToString(rule.SelectToken("Response"))).SelectToken("Response");
                    //return new ObjectResult(Convert.ToString(response.SelectToken("Body")).Replace(_requestContent, false)) { StatusCode = Convert.ToInt32(response.SelectToken("StatusCode")) };
            }
            return null;
        }

        public Response GetResponse(JToken rule)
        {
            var response = _responsesConf.First(p => Convert.ToString(p.SelectToken("Name")) == Convert.ToString(rule.SelectToken("Response"))).SelectToken("Response");
            return new Response() { StatusCode = Convert.ToInt32(response.SelectToken("StatusCode")), Body = Convert.ToString(response.SelectToken("Body")).Replace(_requestContent, false) };
        }

        private void MapperContract(JToken input, Dictionary<string, object> receive)
        {
            var children = input.Children<JToken>();

            foreach(var item in children)
            {
                if(item.HasValues && item.First != null)
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
