using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Camaleao.Api.Models
{
    public class TemplateRequestModel
    {
        [Required(ErrorMessageResourceName ="RequestRequired",ErrorMessageResourceType =typeof(ValidationMessageCatalog))]
        public dynamic Request { get; set; }
        [Required(ErrorMessageResourceName = "ResponseRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public List<ResponseModel> Responses { get; set; }
        [JsonRequired()]
        public List<RuleModel> Rules { get; set; }
        public ContextModel Context { get; set; }
    }

    public class ResponseModel
    {
        public List<ActionModel> Actions { get; set; }
        public string ResponseId { get; set; }
        public int StatusCode { get; set; }
        public dynamic Body { get; set; }
    }

    public class RuleModel
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }
    }

    public class ActionModel
    {
        public string Execute { get; set; }
    }

    public class ContextModel
    {
        public List<VariableModel> Variables { get; set; }
    }

    public class VariableModel
    {
        public string Name { get; set; }
        public dynamic Initialize { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
