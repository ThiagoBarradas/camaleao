using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services
{
    public class TemplateSevice : ITemplateService
    {
        ITemplateRepository _templateRepository;
        public TemplateSevice(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }
        public void Add(Template template)
        {

            template.Id = Guid.NewGuid().ToString();

            _templateRepository.Add(template);
        }
    }
}
