using Bogus;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models.Exemplos {
    public class CreateTemplateExemple : IExamplesProvider {
        public object GetExamples() {

            var request = new Faker<RequestModel>()
                .RuleFor(p => p.Body, new {
                    transaction = new {
                        amount = 100,
                        automaticCapture = true,
                        card = new {
                            brand = "visa",
                            number = "411111111111"
                        }
                    }
                })
                .Ignore(p => p.QueryString)
                .Ignore(p => p.Headers);

            var action = new Faker<ActionModel>()
                .RuleFor(c => c.Execute, "_context.{{notifie_url}}={{NotifieUrl}}");

            var routeModel = new Faker<RouteModel>()
                .RuleFor(c => c.Method, "POST")
                .RuleFor(c => c.Name, "authorize")
                .RuleFor(c => c.Version, "v1");

            var response = new Faker<ResponseModel>()
                .RuleFor(c => c.ResponseId, "authorized")
                .RuleFor(c => c.StatusCode, 201)
                .RuleFor(c => c.Body, new {
                    amount = "_context.{{AmountInCents}}",
                });

            var rule = new Faker<RuleModel>()
                .RuleFor(c => c.Expression, "{{card.number}}=='4000000000000028'")
                .RuleFor(c => c.ResponseId, "authorized");

            var createTemplateRequestModel = new Faker<CreateTemplateRequestModel>()
                .RuleFor(c => c.Route, routeModel)
                .RuleFor(c => c.Request, request)
                .RuleFor(c => c.Actions, action.Generate(1).ToList())
                .RuleFor(c => c.Responses, response.Generate(1).ToList())
                .RuleFor(c => c.Rules, rule.Generate(1).ToList());


            return createTemplateRequestModel;
        }
    }
}
