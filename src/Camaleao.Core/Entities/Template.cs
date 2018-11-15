using Flunt.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Entities
{
    public class Template : Notifiable
    {
        public Template(Guid id) : this()
        {
            this.Id = id;

        }
        public Template()
        {
            this.ResponsesId = new List<Guid>();
            this.Rules = new List<RuleTemplate>();
            this.Actions = new List<Action>();
            this.Variables = new List<Variable>();
        }
        public Guid Id { get; set; }
        public string User { get; set; }
        public RouteTemplate Route { get; set; }
        public RequestTemplate Request { get; set; }
        public List<Guid> ResponsesId { get; private set; }
        public ContextTemplate Context { get; private set; }
        public List<RuleTemplate> Rules { get; set; }
        public List<Action> Actions { get; set; }
        public List<Variable> Variables { get; set; }

        public bool IsValid()
        {
            bool result = true;

            if (this.Request == null)
            {
                AddNotification("Request", "[request] is required in template");
                return false;
            }

            if (this.Route == null)
            {
                AddNotification("Route", "[route] is required in template");
                return false;
            }

            if (!this.Route.IsValid())
            {
                AddNotifications(this.Route.Notifications);
                return false;
            }

            if (!this.Request.IsValid())
            {
                AddNotifications(this.Request.Notifications);
                return false;
            }

            if (this.Rules.Any(p => !p.IsValid(this)))
            {
                this.Rules.ForEach(p => AddNotifications(p.Notifications));
                return false;
            }

            if(Context==null && Request is PostRequestTemplate)
            {
                var requestContainsContext = ((PostRequestTemplate)Request).HasContext();
                if (!requestContainsContext && Actions.Any(p => p.UseContext()))
                {
                    AddNotification("Context", "Your request is doing reference to context, but there isn't mapped context in your template");
                    return false;
                }
                if(!requestContainsContext && Rules.Any(p => p.UseContext()))
                {
                    AddNotification("Context", "Your rules are doing reference to context, but there isn't mapped context in your template");
                    return false;
                }
            }

            return result;
        }

        public void AddContext(ContextTemplate context)
        {
            context.BuildVaribles();
        }

        public void AddResponses(List<ResponseTemplate> responseTemplates)
        {
            this.ResponsesId = responseTemplates.Select(p => p.Id).ToList();
        }
    }
}
