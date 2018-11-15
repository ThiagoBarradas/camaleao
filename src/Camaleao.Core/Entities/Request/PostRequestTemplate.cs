using Camaleao.Core.Enuns;
using Camaleao.Core.Mappers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Entities
{
    public class PostRequestTemplate : RequestTemplate
    {
        public PostRequestTemplate(dynamic body)
        {
            this.Body_ = JsonConvert.SerializeObject(body);
        }
        public string Body_ { get; set; }
        [BsonIgnore()]
        public dynamic Body {
            get {
                return JsonConvert.DeserializeObject<dynamic>(Body_);
            }
        }

        public override bool IsValid()
        {
            var bodyMapped = GetBodyMapped();
            var result = bodyMapped.Values.All(p => VariableTypeEnum.GetValues().Any(c => c.Equals(Convert.ToString(p), comparisonType: StringComparison.InvariantCultureIgnoreCase)));
            if (!result)
                AddNotification("Request", "[request] one or more properties are with wrong type. Allowed types 'integer','text', 'bool', 'context'.");
            return result;
        }

        public Dictionary<string, dynamic> GetBodyMapped()
        {
            var body = JsonConvert.DeserializeObject<JObject>(Body_);
            return body.MapperContractFromObject();
        }
        public bool UseContext()
        {
            var bodyMapped = GetBodyMapped();
            return bodyMapped.Values.Any(p => VariableTypeEnum.Context.Equals(Convert.ToString(p), comparisonType: StringComparison.InvariantCultureIgnoreCase));
        }
        public bool UseExternalContext() {
            var bodyMapped = GetBodyMapped();
            return bodyMapped.Values.Any(p => VariableTypeEnum.ExternalContext.Equals(Convert.ToString(p), comparisonType: StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
