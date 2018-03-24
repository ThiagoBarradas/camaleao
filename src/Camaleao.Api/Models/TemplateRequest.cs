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
        public IList<Response> Responses { get; set; }
        [JsonRequired()]
        public IList<Rule> Rules { get; set; }
    }

    
    public class Response
    {
        public string ResponseId { get; set; }
        public int StatusCode { get; set; }
        public string Header { get; set; }
        public dynamic Body { get; set; }
    }

    public class Rule
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }
    }
}
