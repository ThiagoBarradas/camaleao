using Camaleao.Core.ExtensionMethod;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        public void ValidateContract(string route, dynamic request)
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
            var responsesConf = (JArray)camaleao.SelectToken("Responses");
            var rulesConf = (JArray)camaleao.SelectToken("Rules");
            #endregion

            MapperContract(requestConf, _requestTemplate);
            MapperContract(requestInput, _requestContent);

            if(!_requestTemplate.All(k => _requestContent.ContainsKey(k.Key) && _requestContent[k.Key].GetType() == k.Value.ToString().GetTypeChameleon()))
                AddNotification("BadRequest", "The request don't reflect the contract");
            else
                _requestContent = _requestContent.ConvertType(_requestTemplate);
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
    }
}
