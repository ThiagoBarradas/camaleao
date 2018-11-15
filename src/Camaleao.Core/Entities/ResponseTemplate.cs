using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Camaleao.Core.Entities
{
    public class ResponseTemplate
    {
        public ResponseTemplate()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
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
