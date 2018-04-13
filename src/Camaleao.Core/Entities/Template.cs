using Flunt.Notifications;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Entities
{
    public class Template : Notifiable
    {
        public Template()
        {
            this.Id = Guid.NewGuid().ToString();
            this._responses = new List<ResponseTemplate>();
            this.Rules = new List<RuleTemplate>();
            this.Actions = new List<Action>();
        }
        public string Id { get; set; }
        public string User { get; set; }
        public Route Route { get; set; }
        public RequestTemplate Request { get; set; }

        List<ResponseTemplate> _responses;
        [BsonIgnore]
        public List<ResponseTemplate> Responses {
            get {
                return _responses;
            }
            set {
                value.ForEach(resp => resp.TemplateId = this.Id);
                this._responses = value;

            }
        }

        private ContextTemplate _context;
        public ContextTemplate Context {
            get { return _context; }
            set {
                value?.BuildVaribles();
                _context = value;
            }
        }
        public List<RuleTemplate> Rules { get; set; }
        public List<Action> Actions { get; set; }


        public bool IsValid()
        {
            bool result = true;

            if (this.Request == null)
            {
                AddNotification("Context", "Request is Required in Template");
                return false;
            }

            if (this.Context == null && this.Request.Body != null && !this.Request.Body_.Contains("_context"))
            {
                if (this.Actions.Any(p => p.Execute.Contains("_context")))
                {
                    AddNotification("Context", "Your request is doing reference to context, but there isn't mapped context in your template");
                    result = false;
                }
                try
                {
                    if (JsonConvert.SerializeObject(this.Responses).Contains("_context"))
                    {
                        AddNotification("Context", "Your responses are doing reference to context, but there isn't mapped context in your template");
                        result = false;
                    }
                }
                catch (Newtonsoft.Json.JsonSerializationException js)
                {
                    AddNotification("Context", "Request Body is not a valid json");
                    result = false;
                }

                if (JsonConvert.SerializeObject(this.Rules).Contains("_context"))
                {
                    AddNotification("Context", "Your rules are doing reference to context, but there isn't mapped context in your template");
                    result = false;
                }
            }

            return result;
        }
    }
}
