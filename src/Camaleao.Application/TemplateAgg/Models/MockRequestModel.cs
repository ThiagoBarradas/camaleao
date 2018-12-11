using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models {
    public class MockRequestModel {

        public string Version { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public string User { get; set; }
        public HttpContext HttpContext { get; set; }

        
    }
}
