using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Models
{
    public class ContextModel
    {
        public List<VariableModel> Variables { get; set; }

        public string BuildVariables() {
            var variablesToMap = JsonConvert.SerializeObject(Variables).Replace("\\", string.Empty);

            Variables.ForEach(variable => {
                if (variable.Type == "text") {
                    variablesToMap = variablesToMap.Replace($"'{variable.Value}'", variable.Value);
                }
                else {
                    variablesToMap = variablesToMap.Replace($"\"{variable.Value}\"", variable.Value);
                }
            });

            return variablesToMap;
        }
    }
}
