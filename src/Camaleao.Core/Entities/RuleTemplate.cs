using Flunt.Notifications;
using System.Collections.Generic;

namespace Camaleao.Core.Entities {
    public class RuleTemplate : Notifiable {

        private RuleTemplate() {

        }

        public RuleTemplate(string expression, string responseId, List<ActionTemplate> actions, string name) {
            this.ResponseId = responseId;
            this.Expression = expression;
            this.Actions = actions;
            this.Name = name;
        }

        public string Name { get; set; }
        public List<ActionTemplate> Actions { get; private set; }
        public string Expression { get; private set; }
        public string ResponseId { get; private set; }

        public PostbackTemplate Postback { get; private set; }

        public bool UseContext() {
            return this.Expression.Contains(Enuns.VariableTypeEnum.Context);
        }

        public void AddPostback(PostbackTemplate postback) {
            this.Postback = postback;
        }
        public bool IsValid() {
            return true;
        }

    }

}
