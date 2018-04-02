using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities
{
    public class Context
    {

        public Context()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public List<Variable> Variables { get; set; }

        public string GetVariableWithString()
        {
            StringBuilder retorno = new StringBuilder();

            this.Variables.ForEach(variable =>
            {
                retorno.Append($"var {variable.Name} = {variable.Value};");
            });

            return retorno.ToString();
        }
    }
}
