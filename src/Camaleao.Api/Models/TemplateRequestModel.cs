using Camaleao.Core.ExtensionMethod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Camaleao.Api.Models
{
    public class TemplateRequestModel
    {
        public TemplateRequestModel()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        [JsonRequired()]

        [Required(ErrorMessageResourceName = "RouteRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public RouteModel Route { get; set; }
        [Required(ErrorMessageResourceName = "RequestRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public RequestModel Request { get; set; }
        [Required(ErrorMessageResourceName = "ResponseRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public List<ResponseModel> Responses { get; set; }
        [JsonRequired()]
        public List<RuleModel> Rules { get; set; }
        public ContextModel Context { get; set; }
        public List<ActionModel> Actions { get; set; }
    }

    public class RouteModel
    {
        [Required(ErrorMessageResourceName = "VersionRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public string Version { get; set; }
        [Required(ErrorMessageResourceName = "NameRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public string Name { get; set; }
        [Required(ErrorMessageResourceName = "MethodRequired", ErrorMessageResourceType = typeof(ValidationMessageCatalog))]
        public string Method { get; set; }
    }

    public class ResponseModel
    {
        [JsonProperty("actions")]
        public List<ActionModel> Actions { get; set; }
        public string ResponseId { get; set; }
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }
        [JsonProperty("body")]
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

        public string BuildVariables()
        {
            var variablesToMap = JsonConvert.SerializeObject(Variables).Replace("\\", string.Empty);

            Variables.ForEach(variable =>
            {
                if (variable.Type == "text")
                {
                    variablesToMap = variablesToMap.Replace($"'{variable.Value}'", variable.Value);
                }
                else
                {
                    variablesToMap = variablesToMap.Replace($"\"{variable.Value}\"", variable.Value);
                }
            });

            return variablesToMap;
        }
    }

    public class VariableModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public dynamic Initialize { get; set; }
        public string Value { get; set; }
    }


    public class RequestModel
    {
        [JsonProperty("headers")]
        public List<KeyValuePair<string, string>> Headers { get; set; }
        [JsonProperty("body")]
        public dynamic Body { get; set; }
        [JsonProperty("query_string")]
        public string QueryString { get; set; }
    }
}
