using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities
{
    public class RequestTemplate
    {

        public RequestTemplate()
        {
            this.Headers = new List<KeyValuePair<string, string>>();
        }
        public IList<KeyValuePair<string,string>> Headers { get; set; }
        public string Body_ { get; set; }
        [BsonIgnore()]
        public dynamic Body {
            get {
                return JsonConvert.DeserializeObject<dynamic>(Body_);
            }
            set {
                this.Body_ = JsonConvert.SerializeObject(value);
            }
        }
        public string QueryString { get; set; }
    }
}
