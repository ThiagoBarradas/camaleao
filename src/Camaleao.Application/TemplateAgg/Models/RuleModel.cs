using System.Collections.Generic;

namespace Camaleao.Application.TemplateAgg.Models {
    public class RuleModel {

        public string Name { get; set; }
        public string Expression { get; set; }
        public string ResponseId { get; set; }
        public Postback Postback { get; set; }
        public List<ActionModel> Actions { get; set; }
    }
}
