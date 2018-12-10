using Flunt.Notifications;
using System;
using System.Collections.Generic;

namespace Camaleao.Core.Entities {
    public class ContextTemplate: Notifiable {
        private ContextTemplate()
        {
            this.Id = Guid.NewGuid();
        }

        public ContextTemplate(List<Variable> variables):this() {
            this.Variables = variables;
        }
        public Guid Id { get; private set; }
        public string Indetifier { get; private set; }
        public List<Variable> Variables { get; private set; }
        public string User { get; private set; }

        public Context CreateContext(string externalIdentifier)
        {
            return new Context()
            {
                Variables = this.Variables,
                User=User,
                ExternalIdentifier=externalIdentifier
            };
        }

        public void AddUser(string user) {
            this.User = user;
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
