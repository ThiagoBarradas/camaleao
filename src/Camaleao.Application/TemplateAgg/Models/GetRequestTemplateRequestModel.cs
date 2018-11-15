using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models {
    public class GetRequestTemplateRequestModel {

        //string user, string version, string routeName, string method

        public string User { get; set; }
        public string Version { get; set; }
        public string RouteName { get; set; }
        public string Method { get; set; }
    }
}
