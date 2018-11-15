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

        public bool Validate(Template template) {

            if (template.Context == null && template.Request is PostRequestTemplate) {
                var requestContainsContext = ((PostRequestTemplate)template.Request).HasContext();
                if (!requestContainsContext && template.Actions.Any(p => p.UseContext())) {
                    AddNotification("Context", "Your request is doing reference to context, but there isn't mapped context in your template");
                    return false;
                }
                if (!requestContainsContext && template.Rules.Any(p => p.UseContext())) {
                    AddNotification("Context", "Your rules are doing reference to context, but there isn't mapped context in your template");
                    return false;
                }
            }

            foreach (var rule in template.Rules) {
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
                hasContext = postRequestTemplate.HasContext();

                foreach (var key in requestMapped.Keys) {
                    var variable = $"{{{{{key}}}}}";
                    if (expression.Contains(variable)) {
                        var value = Convert.ToString(requestMapped[key]);
                        expression = expression.Replace(variable, Enuns.VariableTypeEnum.GetMockValueByType(value).ToString().ToLower());
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
                this.engineService.Execute<bool>(rule.Expression);
            }
            catch {
                AddNotification("Rule", string.Format("[Rule]({0}) contains an invalid logical expression", rule.Expression));
                return false;
            }
            return true;
        }

    }
}
