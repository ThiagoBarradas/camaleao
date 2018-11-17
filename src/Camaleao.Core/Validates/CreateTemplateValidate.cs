using Camaleao.Core.Entities;
using Camaleao.Core.Repository;
using Camaleao.Core.SeedWork;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Validates {
    public class CreateTemplateValidate : Notifiable, ICreateTemplateValidate {

        private readonly IEngineService engineService;
        private readonly ITemplateRepository templateRepository;
        public CreateTemplateValidate(IEngineService engineService, ITemplateRepository templateRepository) {
            this.engineService = engineService;
            this.templateRepository = templateRepository;

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
                hasContext = template.Context!=null;

                foreach (var key in requestMapped.Keys) {
                    var variable = $"{{{{{key}}}}}";
                    if (expression.Contains(variable)) {
                        var value = Convert.ToString(requestMapped[key]);
                        expression = expression.Replace(variable, Enuns.VariableTypeEnum.GetMockValueByType(value));
                    }
                }

                if (hasContext)
                    foreach (var variable in template.Context.Variables) {
                        expression = ReplateExpressionFromVariable(expression, variable);
                    }
                else {
                    ReplaceExpressionFromContext(ref expression, template.User);
                }
            }


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

        private static string ReplateExpressionFromVariable(string expression, Variable variable) {
            var variableInContext = $"_context.{{{{{variable.Name}}}}}";
            if (expression.Contains(variableInContext)) {
                expression = expression.Replace(variableInContext, Enuns.VariableTypeEnum.GetMockValueByType(variable.Type).ToString().ToLower());
            }

            return expression;
        }

        private void ReplaceExpressionFromContext(ref string expression, string user) {
            var variables = ExpressionUtility.ExtractVariables(expression).ToArray();
            var templates = this.templateRepository.Get(p => p.User == user);
            foreach(var template in templates) {
                if (template.Context != null) {
                    var contextVariables = template.Context.Variables.Where(p => variables.Contains(p.Name)).ToList();
                    foreach (var variable in contextVariables) {
                        expression = ReplateExpressionFromVariable(expression, variable);
                    }
                }   
            }
        }

    }
}
