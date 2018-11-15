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

        public bool IsValid(Template template)
        {
            string expression = this.Expression;
            if (template.Request is PostRequestTemplate)
            {
                var requestMapped = ((PostRequestTemplate)template.Request).GetBodyMapped();
                foreach (var key in requestMapped.Keys)
                {
                    var variable = $"{{{{{key}}}}}";
                    if (expression.Contains(variable))
                    {
                        var value = Convert.ToString(requestMapped[key]);
                        expression = expression.Replace(variable, Enuns.VariableTypeEnum.GetMockValueByType(value).ToString().ToLower());
                    }
                }
                foreach(var variable in template.Variables)
                {
                    var variableInContext = $"_context.{{{{{variable}}}}}";
                    if (expression.Contains(variableInContext))
                    {
                        expression = expression.Replace(variableInContext, Enuns.VariableTypeEnum.GetMockValueByType(variable.Type).ToString().ToLower());
                    }
                }

                if (expression.Contains("{{"))
                {
                    AddNotification("Rule",string.Format("[Rule]({0}) contains an invalid request path or undeclared variable.", this.Expression));
                    return false;
                }

            }

            return true;
        }

    }

}
