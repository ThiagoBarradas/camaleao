using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities
{
    public class ContextTemplate: Notifiable {
        public ContextTemplate()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
        public List<Variable> Variables { get; set; }
        public string User { get; set; }

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

        internal bool IsValid() {
          
            foreach(var variable in this.Variables) {
                if (!variable.IsValid()) {
                    this.AddNotifications(variable.Notifications);
                    return false;
                }
            }
            return true;
        }
    }
}
