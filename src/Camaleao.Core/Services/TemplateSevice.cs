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

namespace Camaleao.Core.Services {
    public class TemplateService : Notifiable, ITemplateService {
        readonly ITemplateRepository _templateRepository;

        public TemplateService(ITemplateRepository templateRepository) {
            _templateRepository = templateRepository;
        }

        public void Add(Template template) =>
            _templateRepository.Add(template);


        public List<Template> Find(Expression<Func<Template, bool>> expression) =>
            _templateRepository.Get(expression);

        public Template FindByRoute(string user, RouteTemplate route) {
            return _templateRepository.Get(p => p.User == user &&
                    p.Route.Version == route.Version &&
                    p.Route.Name == route.Name &&
                     p.Route.Method == route.Method).FirstOrDefault();
        }

        public void Remove(Template template) =>
            _templateRepository.Remove(p => p.Id == template.Id);

        public void Update(Template template) {
            _templateRepository.Update(p => p.Id == template.Id, template);
        }


    }
}
