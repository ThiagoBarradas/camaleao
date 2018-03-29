using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using Jint;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Camaleao.Core.Services
{
    public class EngineService : IEngineService
    {
        private readonly Engine _engine;
        private readonly IScriptRepository _scriptRepository;

        public EngineService(Engine engine, IScriptRepository scriptRepository)
        {
            _engine = engine;
            _scriptRepository = scriptRepository;
            LoadPreScript();
        }

        private void LoadPreScript()
        {
            StringBuilder script = new StringBuilder();
            _scriptRepository.GetAll().Result.ForEach(p => script.AppendLine(p.Script));
            _engine.Execute(script.ToString());
        }

        public void LoadRequest(JObject request, string variavel)
        {
            _engine.Execute($"{variavel} = {request}");
        }

        public T Execute<T>(string expression)
        {
            var result = _engine.Execute(expression).GetCompletionValue().ToString();
            return (T)Convert.ChangeType(result, Type.GetType(typeof(T).FullName, false, true));
        }
    }
}
