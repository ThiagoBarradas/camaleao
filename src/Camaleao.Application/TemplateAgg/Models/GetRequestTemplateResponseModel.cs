using System;
using System.Collections.Generic;

namespace Camaleao.Application.TemplateAgg.Models
{
    public class GetRequestTemplateResponseModel {

        public Guid Token { get; set; }
        public TemplateResponseModel Template { get; set; }
    }

    public class TemplateResponseModel
    {
        public RouteModel Route { get; set; }
        public RequestModel Request { get; set; }
        public List<ActionModel> Actions { get; set; }
        public List<ResponseModel> Responses { get; set; }
        public List<RuleModel> Rules { get; set; }
        public ContextModel Context { get; set; }
    }
}
