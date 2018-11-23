using Flunt.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Entities {
    public class Template : Notifiable {
        public Template(string user) : this() {
     

        }

        public Template(Guid id):this() {
            this.Id = id;
        }
        private Template() {
            this.Id = Guid.NewGuid();
            this.ResponsesId = new List<Guid>();
            this.Rules = new List<RuleTemplate>();
            this.Actions = new List<ActionTemplate>();
        }
        public Guid Id { get; private set; }
        public string User { get; private set; }
        public RouteTemplate Route { get; set; }
        public RequestTemplate Request { get; set; }
        public List<Guid> ResponsesId { get; private set; }
        public ContextTemplate Context { get; private set; }
        public List<RuleTemplate> Rules { get; set; }
        public List<ActionTemplate> Actions { get; set; }

        public bool IsValid() {
            bool result = true;

            if (this.Request == null) {
                AddNotification("Request", "[request] is required in template");
                return false;
            }

            if (this.Route == null) {
                AddNotification("Route", "[route] is required in template");
                return false;
            }

            if (!this.Route.IsValid()) {
                AddNotifications(this.Route.Notifications);
                return false;
            }

            if (!this.Request.IsValid()) {
                AddNotifications(this.Request.Notifications);
                return false;
            }

            if (this.Rules.Any(p => !p.IsValid())) {
                this.Rules.ForEach(p => AddNotifications(p.Notifications));
                return false;
            }

            if (this.Context != null && !this.Context.IsValid()) {
                AddNotifications(this.Context.Notifications);
                return false;
            }

            return result;
        }
      
        public void AddResponses(List<ResponseTemplate> responseTemplates) {
            this.ResponsesId = responseTemplates.Select(p => p.Id).ToList();
        }

        public void AddUser(string user) {
            this.User = user;
            if (this.Context != null)
                this.Context.AddUser(user);
        }
    }
}
