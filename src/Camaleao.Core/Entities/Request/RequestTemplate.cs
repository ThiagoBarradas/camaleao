using Flunt.Notifications;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Camaleao.Core.Entities {
    [BsonKnownTypes(typeof(PostRequestTemplate), typeof(GetRequestTemplate))]
    public abstract class RequestTemplate: Notifiable {
        public RequestTemplate() {
            this.Headers = new List<KeyValuePair<string, string>>();
        }
        public IList<KeyValuePair<string, string>> Headers { get; set; }

        public abstract bool IsValid();
    }
}
