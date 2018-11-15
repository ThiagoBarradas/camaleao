using Camaleao.Core.Entities;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Validates {
    public class CreateTemplateValidate : Notifiable, ICreateTemplateValidate {

        private IEngineService engineService;
        public CreateTemplateValidate(IEngineService engineService) {
            this.engineService = engineService;
        }

        public bool Validate(Template template, IList<ResponseTemplate> responses) {

            if (template.Request is PostRequestTemplate) {

                var requestContainsContext = ((PostRequestTemplate)template.Request).UseContext();
                var requestContainsExternalContext = ((PostRequestTemplate)template.Request).UseExternalContext();

                if (!requestContainsContext && !requestContainsExternalContext && template.Actions.Any(p => p.UseContext())) {
                    AddNotification("Context", "Your request is doing reference to context, but there isn't mapped context in your template");
                    return false;
                }
                if (!requestContainsContext && !requestContainsExternalContext && template.Rules.Any(p => p.UseContext())) {
                    AddNotification("Context", "Your rules are doing reference to context, but there isn't mapped context in your template");
                    return false;
                }
            }

            foreach (var rule in template.Rules) {
                if (!responses.Any(p => string.Compare(p.ResponseId, rule.ResponseId, true) == 0)) {
                    AddNotification("Rule", string.Format("[rule]({0}) contains invalid [response_id].", rule.Expression));
                    return false;
                }

                var result = this.ValidateRule(template, rule);
                if (!result)
                    return false;
            }

            return true;
        }

        public IReadOnlyCollection<Notification> GetNotifications() {
            return this.Notifications;
        }

        private bool ValidateRule(Template template, RuleTemplate rule) {
            string expression = rule.Expression;
            bool hasContext = false;

            if (template.Request is PostRequestTemplate) {
                var postRequestTemplate = ((PostRequestTemplate)template.Request);
                var requestMapped = postRequestTemplate.GetBodyMapped();
                hasContext = postRequestTemplate.UseContext();

                foreach (var key in requestMapped.Keys) {
                    var variable = $"{{{{{key}}}}}";
                    if (expression.Contains(variable)) {
                        var value = Convert.ToString(requestMapped[key]);
                        expression = expression.Replace(variable, Enuns.VariableTypeEnum.GetMockValueByType(value));
                    }
                }

                if (hasContext)
                    foreach (var variable in template.Variables) {
                        var variableInContext = $"_context.{{{{{variable}}}}}";
                        if (expression.Contains(variableInContext)) {
                            expression = expression.Replace(variableInContext, Enuns.VariableTypeEnum.GetMockValueByType(variable.Type).ToString().ToLower());
                        }
                    }
            }

            if (expression.Contains("_context.{{") && hasContext == false)
                return true;

            if (expression.Contains("{{")) {
                AddNotification("Rule", string.Format("[Rule]({0}) contains an invalid request path or undeclared variable.", rule.Expression));
                return false;
            }

            try {
                this.engineService.Execute<bool>(expression);
            }
            catch {
                AddNotification("Rule", string.Format("[Rule]({0}) contains an invalid logical expression", rule.Expression));
                return false;
            }
            return true;
        }

    }
}
