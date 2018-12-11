using Camaleao.Core.Enuns;
using Camaleao.Core.Mappers;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Entities {
    public class PostRequestTemplate : RequestTemplate {
        public PostRequestTemplate(dynamic body) {
            this.Body_ = JsonConvert.SerializeObject(body);
            LoadContextType();
        }

        public string Body_ { get; set; }

        [BsonIgnore()]
        public dynamic Body {
            get {
                return JsonConvert.DeserializeObject<dynamic>(Body_);
            }
        }

        public ContextKey ContextKey { get; private set; }

        public override bool IsValid() {
            var bodyMapped = GetBodyMapped();
            var result = bodyMapped.Values.All(p => VariableTypeEnum.GetValues().Any(c => c.Equals(Convert.ToString(p), comparisonType: StringComparison.InvariantCultureIgnoreCase)));
            if (!result)
                AddNotification("Request", "[request] one or more properties are with wrong type. Allowed types 'integer','text', 'bool', 'context'.");
            return result;
        }

        private Dictionary<string, dynamic> bodyMapped;
        public Dictionary<string, dynamic> GetBodyMapped() {
            if (bodyMapped == null) {
                var body = JsonConvert.DeserializeObject<JObject>(Body_);
                bodyMapped = body.MapperContractFromObject();
            }
            return bodyMapped;
        }

        public bool UseContext() {
            return ContextKey.Type == VariableTypeEnum.Context;
        }

        public bool UseExternalContext() {
            return ContextKey.Type == VariableTypeEnum.ExternalContext;
        }

        private void LoadContextType() {

            foreach (var key in GetBodyMapped().Keys) {

                var value = Convert.ToString(GetBodyMapped()[key]);

                if (VariableTypeEnum.ExternalContext.Equals(value,
         comparisonType: StringComparison.InvariantCultureIgnoreCase)) {
                    ContextKey = new ContextKey(key, VariableTypeEnum.ExternalContext);
                    return;
                }
                else if (VariableTypeEnum.Context.Equals(value,
                    comparisonType: StringComparison.InvariantCultureIgnoreCase)) {
                    ContextKey = new ContextKey(key, VariableTypeEnum.Context);
                    return;
                }

            }
            ContextKey = new ContextKey(string.Empty, VariableTypeEnum.None);
        }

        public static PostRequestTemplate CreatePostRequestFromPost(dynamic body) {

            JObject jBody = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(body));

            ReplaceType(jBody.Children().OfType<JProperty>());

            return new PostRequestTemplate(jBody);

        }

        private static void ReplaceType(IEnumerable<JProperty> tokens) {

            foreach (JProperty token in tokens) {

                if (token.Value.Children().Any())
                    ReplaceType(token.Value.Children().OfType<JProperty>());
                else {
                    switch (token.Value.Type) {
                        case JTokenType.Boolean:
                            token.Value = VariableTypeEnum.Boolean;
                            break;
                        case JTokenType.Bytes:
                        case JTokenType.Float:
                        case JTokenType.Integer:
                            token.Value = VariableTypeEnum.Integer;
                            break;
                        case JTokenType.String:
                            token.Value = VariableTypeEnum.Text;
                            break;
                        default:
                            token.Value = VariableTypeEnum.Text;
                            break;
                    }
                }
            }
        }



    }

    public class ContextKey {

        public ContextKey(string key, string type) {
            this.Type = type;
            this.Key = key;
        }
        public string Type { get; private set; }
        public string Key { get; private set; }
    }
}
