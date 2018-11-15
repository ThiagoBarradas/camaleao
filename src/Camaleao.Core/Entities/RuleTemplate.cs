using Flunt.Notifications;
using System;

namespace Camaleao.Core.Entities
{
    public class RuleTemplate : Notifiable
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }

        public PostbackTemplate Postback { get; set; }

        public bool UseContext()
        {
            return this.Expression.Contains(Enuns.VariableTypeEnum.Context);
        }

        public bool IsValid()
        {
            return true;
        }

    }

}
