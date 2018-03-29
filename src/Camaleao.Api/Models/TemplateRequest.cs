using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Camaleao.Api.Models
{
    public class TemplateRequest
    {
        [Required(ErrorMessageResourceName ="RequestRequired",ErrorMessageResourceType =typeof(ValidationMessageCatalog))]
        public dynamic Request { get; set; }
        [Required(ErrorMessageResourceName = "ResponseRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public List<Response> Responses { get; set; }
        [JsonRequired()]
        public List<Rule> Rules { get; set; }
    }

    
    public class Response
    {
        public string ResponseId { get; set; }
        public Callback Callback { get; set; }
        public string Expression { get; set; }
        public List<string> Events { get; set; }
        public int StatusCode { get; set; }
        public string Header { get; set; }
        public dynamic Body { get; set; }
    }

    public class Callback
    {
        public string ResponseId { get; set; }
        public string Variables { get; set; }
    }

    public class Rule
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }
    }
}
