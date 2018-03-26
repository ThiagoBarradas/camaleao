using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Camaleao.Core.Services
{
    public class TemplateSevice : ITemplateService
    {
        readonly ITemplateRepository _templateRepository;
        public TemplateSevice(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }
        public void Add(Template template)
        {

            template.Id = Guid.NewGuid().ToString();

            _templateRepository.Add(template);
        }

        public List<Template> Find(Expression<Func<Template, bool>> expression)
        {
            return _templateRepository.Get(expression).Result;
        }

        public Template FirstOrDefault(Expression<Func<Template, bool>> expression)
        {
            return _templateRepository.Get(expression).Result.FirstOrDefault();
        }
    }
}
