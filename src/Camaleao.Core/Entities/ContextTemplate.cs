using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities
{
    public class ContextTemplate
    {
        public ContextTemplate()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
        public List<Variable> Variables { get; set; }

        public Context CreateContext()
        {
            return new Context()
            {
                Variables = this.Variables
            };
        }

        public void BuildVaribles()
        {
            this.Variables.ForEach(variable => variable.BuildVariable());
        }
    }
}
