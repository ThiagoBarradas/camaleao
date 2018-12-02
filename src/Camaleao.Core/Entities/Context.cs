using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities
{
    public class Context: Entity {

       
        public string ExternalIdentifier { get; set; }
        public List<Variable> Variables { get; set; }

        public string GetVariablesAsString()
        {
            StringBuilder retorno = new StringBuilder();

            this.Variables.ForEach(variable =>
            {
                retorno.Append($"var {variable.Name} = {variable.Value};");
            });

            retorno.Append("var _context=true;");
            return retorno.ToString();
        }

        public override bool IsValid() {
            return true;
        }
    }
}
