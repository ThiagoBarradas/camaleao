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

        public IList<ResponseSettings> Responses { get; set; }
        public IList<Rule> Rules { get; set; }
    }

    public class ResponseSettings
    {
        public string Id { get; set; }
        public Response Response { get; set; }
    }

    public class Response
    {
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
}
