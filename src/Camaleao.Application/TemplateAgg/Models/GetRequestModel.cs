using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models {
    public class GetRequestModel {

        public string User { get; set; }
        public RouteTemplate Route { get; set; }
        public string QueryString { get; set; }
    }
}
