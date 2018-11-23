using Flunt.Notifications;
using System;

namespace Camaleao.Core.Entities {
    public class RuleTemplate : Notifiable {

        private RuleTemplate() {

        }

        public RuleTemplate(string expression, string responseId) {
            this.ResponseId = responseId;
            this.Expression = expression;
        }

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
