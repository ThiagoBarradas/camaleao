using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities
{
    public class ResponseTemplate
    {
        public Guid Id { get; set; }
        public string TemplateId { get; set; }
        public List<Action> Actions { get; set; }
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
}
