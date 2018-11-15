using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models
{
    public class ResponseModel { 
        public List<ActionModel> Actions { get; set; }
        public string ResponseId { get; set; }
        public int StatusCode { get; set; }
        public dynamic Body { get; set; }
    }
}
