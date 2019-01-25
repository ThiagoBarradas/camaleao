using Bogus;
using Swashbuckle.AspNetCore.Examples;

namespace Camaleao.Application.TemplateAgg.Models.Exemplos {
    public class CreateTemplateResponseModelSample : IExamplesProvider {
        public object GetExamples() {
          
        return new Faker<CreateTemplateResponseModel>()
                .RuleFor(p => p.Route, "")
                .RuleFor(p => p.Method, "POST")
                .RuleFor(p => p.Token, "7b53ce02-8a09-44e0-8d71-19c75f107143");
        }
    }
}
