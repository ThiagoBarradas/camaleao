using System.Collections.Generic;

namespace Camaleao.Application.TemplateAgg.Models {
    public class Postback {
        public byte DelayInSeconds { get; set; }
        public string ResponseId { get; set; }
        public string Url { get; set; }
        public List<ActionModel> Actions { get; set; }

    }
}
