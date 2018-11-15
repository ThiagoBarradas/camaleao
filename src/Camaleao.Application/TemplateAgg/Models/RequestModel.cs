using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models {
    public class RequestModel {
        public List<KeyValuePair<string, string>> Headers { get; set; }
        public dynamic Body { get; set; }
        public string QueryString { get; set; }
    }
}
