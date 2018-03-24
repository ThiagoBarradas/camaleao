using Camaleao.Core.Services.Interfaces;
using Jint;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Camaleao.Core.Services
{
    public class EngineService : IEngineService
    {
        private Engine _engine;

        public EngineService(Engine engine)
        {
            _engine = engine;
            LoadPreScript();
        }

        private void LoadPreScript()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Pock");
            var scripts = File.ReadAllText(Path.Combine(path, "script.js"));
            _engine.Execute(scripts);
        }

        public void LoadRequest(JObject request)
        {
            _engine.Execute($"_request = {request}");
        }

        public T Execute<T>(string expression)
        {
            var result = _engine.Execute(expression).GetCompletionValue().ToString();
            return (T)Convert.ChangeType(result, Type.GetType(typeof(T).FullName, false, true));
        }
    }
}
