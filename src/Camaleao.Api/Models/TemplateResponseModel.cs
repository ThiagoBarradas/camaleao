using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Camaleao.Api.Models
{
    public class TemplateResponseModel
    {
        public RouteModel Route { get; set; }
        public dynamic Request { get; set; }
        public List<ActionModel> Actions { get; set; }
        public List<ResponseModel> Responses { get; set; }
        public List<RuleModel> Rules { get; set; }
        public ContextModel Context { get; set; }
    }
}
