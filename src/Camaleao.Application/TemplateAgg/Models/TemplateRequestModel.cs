using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Camaleao.Application.TemplateAgg.Models {
    public abstract class TemplateRequestModel {

        [Required(ErrorMessageResourceName = "RouteRequired")]
        public RouteModel Route { get; set; }
        [Required(ErrorMessageResourceName = "RequestRequired")]
        public RequestModel Request { get; set; }
        public List<ResponseModel> Responses { get; set; }
        [JsonRequired()]
        public List<RuleModel> Rules { get; set; }
        public ContextModel Context { get; set; }
        public List<ActionModel> Actions { get; set; }
    }
}
