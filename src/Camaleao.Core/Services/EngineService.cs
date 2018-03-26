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
        private readonly Engine _engine;
        private readonly IConfigurationService _configurationService;

        public EngineService(Engine engine,IConfigurationService configurationService)
        {
            _engine = engine;
            _configurationService = configurationService;
            LoadPreScript();
        }

        private void LoadPreScript()
        {
           var path = Path.Combine(_configurationService.ServerPath, @"Documents\script.js");
            var scripts = File.ReadAllText(path);
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
