using Camaleao.Core.Mappers;
using Camaleao.Core.SeedWork;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Camaleao.Core.Entities.Request {
    public class PostRequestRecived : RequestRecived {

        public PostRequestRecived(PostRequestTemplate requestTemplate, Stream requestRecived) {
            this.RequestTemplate = requestTemplate;
            this.RequestRecived = requestRecived.DeserializeJson<JObject>();
        }
        public PostRequestTemplate RequestTemplate { get; private set; }
        public JObject RequestRecived { get; private set; }

        private Dictionary<string, dynamic> bodyMapped;
        public Dictionary<string, dynamic> GetBodyMapped() {
            if (bodyMapped == null) {
                bodyMapped = RequestRecived.MapperContractFromObject();
            }
            return bodyMapped;
        }

        public override bool IsValid() {

            var verify = this.RequestTemplate.GetBodyMapped().Keys.All(this.GetBodyMapped().Keys.Contains);
            return verify;

        }

        public override string GetContextIdentifier() {

            if (this.GetBodyMapped().Keys.Contains(RequestTemplate.ContextKey.Key))
                return this.GetBodyMapped()[RequestTemplate.ContextKey.Key];
            else
                return string.Empty;
        }

        public override RequestTemplate GetRequestTemplate() {
            return this.RequestTemplate;
        }
    }
}
