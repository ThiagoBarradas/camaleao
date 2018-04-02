using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Camaleao.Core.Entities
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
        public List<ResponseTemplate> Responses { get; set; }
        public ContextTemplate Context { get; set; }
        public List<RuleTemplate> Rules { get; set; }
    }
}
