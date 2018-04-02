using Camaleao.Core;
using Camaleao.Core.Entities;
using Camaleao.Core.Repository;

namespace Camaleao.Repository
{
    public class TemplateRepository : BaseRepository<Template>, ITemplateRepository
    {
        public TemplateRepository(Settings settings) : base(settings)
        {
        }

        protected override string GetCollectionName()
        {
            return "template";
        }
    }
}
