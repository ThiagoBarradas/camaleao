using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Flunt.Notifications;
using Newtonsoft.Json;
using Camaleao.Core.Entities;

namespace Camaleao.Core.Services
{
    public class TemplateService : Notifiable, ITemplateService
    {
        readonly ITemplateRepository _templateRepository;

        public TemplateService(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public void Add(Template template) =>
            _templateRepository.Add(template);


        public List<Template> Find(Expression<Func<Template, bool>> expression) =>
            _templateRepository.Get(expression).Result;


        public Template FirstOrDefault(Expression<Func<Template, bool>> expression) =>
            _templateRepository.Get(expression).Result.FirstOrDefault();

        public void Remove(Template template) => 
            _templateRepository.Remove(p => p.Id == template.Id);

        public void Update(Template template)
        {
            _templateRepository.Update(p => p.Id == template.Id, template);
        }

        public IReadOnlyCollection<Notification> ValidateTemplate(Template template)
        {
            //  ValidateContext(template);
            return Notifications;
        }

        private void ValidateContext(Template template)
        {
            if (template.Context == null)
            {

                if (template.Request_.Contains("_context"))
                    AddNotification("Context", "Your request is doing reference to context, but there isn't mapped context in your template");

                if (JsonConvert.SerializeObject(template.Responses).Contains("_context"))
                    AddNotification("Context", "Your responses are doing reference to context, but there isn't mapped context in your template");

                if (JsonConvert.SerializeObject(template.Rules).Contains("_context"))
                    AddNotification("Context", "Your rules are doing reference to context, but there isn't mapped context in your template");
            }
        }
    }
}
