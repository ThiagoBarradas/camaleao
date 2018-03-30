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
        public Context Context { get; set; }
    }

    
    public class Response
    {
        public List<Action> Actions { get; set; }
        public string ResponseId { get; set; }
        public int StatusCode { get; set; }
        public dynamic Body { get; set; }
    }

    public class Rule
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }
    }

    public class Action
    {
        public string Execute { get; set; }
    }

    public class Context
    {
        public List<Variable> Variables { get; set; }
    }

    public class Variable
    {
        public string Name { get; set; }
        public string Initialize { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
