using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camaleao.Api.Models
{
    public class TemplateRequest
    {
        
        [JsonRequired()]
        public dynamic Request { get; set; }
        [JsonRequired()]
        public IList<ResponseSettings> Responses { get; set; }
        [JsonRequired()]
        public IList<Rule> Rules { get; set; }

    }


    public class ResponseSettings
    {
        public string Id { get; set; }
        public Response Response { get; set; }
    }
    public class Response
    {
        public int StatusCode { get; set; }
        public dynamic Body { get; set; }
    }

    public class Rule
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }
    }
}
