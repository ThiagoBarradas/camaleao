using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models {
    public class CreateTemplateRequestModel {
    
        [Required(ErrorMessageResourceName = "RouteRequired")]
        public RouteModel Route { get; set; }
        [Required(ErrorMessageResourceName = "RequestRequired")]
        public RequestModel Request { get; set; }
        [Required(ErrorMessageResourceName = "ResponseRequired")]
        public List<ResponseModel> Responses { get; set; }
        [JsonRequired()]
        public List<RuleModel> Rules { get; set; }
        public ContextModel Context { get; set; }
        public List<ActionModel> Actions { get; set; }
    }
}
