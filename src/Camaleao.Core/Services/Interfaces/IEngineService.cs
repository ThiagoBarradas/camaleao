using Newtonsoft.Json.Linq;

namespace Camaleao.Core.Services.Interfaces {
    public interface IEngineService
    {
        T Execute<T>(string expression, bool ignoreError = false);
        void LoadRequest(JObject request, string variavel);
        string VariableType(string variable);
        void SetVariable(string variable, dynamic value, string type);
        void SetVariable(string variable);
    }
}
