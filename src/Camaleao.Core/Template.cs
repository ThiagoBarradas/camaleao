using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core
{
    public class Template
    {
        public string Route { get; set; }
        public string Id { get; set; }

        [BsonIgnore()]
        public dynamic Request {
            get {
                return JsonConvert.DeserializeObject<dynamic>(Request_);
            }
            set {
                this.Request_ = JsonConvert.SerializeObject(value);
            }
        }

        public string Request_ { get; set; }

        [BsonIgnore]
        public List<Response> Responses { get; set; }
        public Context Context { get; set; }
        public List<Rule> Rules { get; set; }
    }

    public class Response
    {
        [BsonId]
        public Guid Id_ { get; set; }
        public string TemplateId { get; set; }
        public List<Action> Actions { get; set; }
        //public List<string> Events { get; set; }
        public string ResponseId { get; set; }
        public int StatusCode { get; set; }

        [BsonIgnore()]
        public dynamic Body {
            get {
                return JsonConvert.DeserializeObject<dynamic>(Body_);
            }
            set {
                this.Body_ = JsonConvert.SerializeObject(value);
            }
        }
        public string Body_ { get; set; }
    }

    public class Rule
    {
        public string Expression { get; set; }
        public string ResponseId { get; set; }
    }

    public class Action
    {
        public string Execute { get; set; }
    }

    public class Context
    {
        public Guid Id { get; set; }
        public string TemplateId { get; set; }
        public List<Variable> Variables { get; set; }
    }

    public class Variable
    {
        public string Name { get; set; }
        public string Initialize { get; set; }
        public string Value { get; set; }
    }

    //public class Event
    //{
    //    [BsonId]
    //    public Guid Id_ { get; set; }
    //    public string EventId { get; set; }
    //    public string Action { get; set; }
    //}
}
