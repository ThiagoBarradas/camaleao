using Camaleao.Core;
using Camaleao.Core.Repository;

namespace Camaleao.Repository
{
    public class ScriptEngineRepository : BaseRepository<ScriptEngine>, IScriptRepository
    {
        public ScriptEngineRepository(Settings settings) : base(settings)
        {
        }

        protected override string GetCollectionName()
        {
            return "Scripts";
        }
    }
}
