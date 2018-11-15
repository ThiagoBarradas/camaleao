using Camaleao.Core.Entities;
using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces {
    public interface ITemplateService {
        void Add(Template template);
        void Update(Template template);
        void Remove(Template template);
        List<Template> Find(Expression<Func<Template, bool>> expression);
        Template FindByRoute(string user, RouteTemplate route);

    }
}
