using Camaleao.Core.Entities;
using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using Jint;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services {
    public class EngineService : IEngineService {
        private readonly Engine _engine;
        private readonly IScriptRepository _scriptRepository;

        public EngineService(Engine engine, IScriptRepository scriptRepository) {
            _engine = engine;
            _scriptRepository = scriptRepository;
            LoadPreScript();
        }

        private void LoadPreScript() {
            StringBuilder script = new StringBuilder();
            _scriptRepository.GetAll().ForEach(p => script.AppendLine(p.Script));
            _engine.Execute(script.ToString());
        }

        public void LoadRequest(JObject request, string variavel) {

            _engine.Execute($"{variavel} = {request}");
        }
        public void SetVariable(string variable, string value) {
            _engine.Execute($"{variable} = '{value}'");
        }

        public T Execute<T>(string expression) {

            try {
                var result = _engine.Execute(expression).GetCompletionValue().ToString();
                return (T)Convert.ChangeType(result, Type.GetType(typeof(T).FullName, false, true));
            }
            catch (Exception ex) {
                using (LogContext.PushProperty("Content", expression)) {
                    Log.Error(ex, "Error in execute script");
                }
                if (typeof(T) == typeof(String))
                    return (T)(object)String.Empty;
                throw ex;
            }

        }

        public string VariableType(string variable) {

            return _engine.Execute($"typeof {variable}").GetCompletionValue().ToString();
        }
    }
}
