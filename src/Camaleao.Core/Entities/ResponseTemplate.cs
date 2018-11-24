using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Camaleao.Core.Entities {
    public class ResponseTemplate {

        public ResponseTemplate(string user, string responseId, int statusCode, dynamic body, List<ActionTemplate> actions) : this() {
            this.User = user;
            this.ResponseId = responseId.ToLower();
            this.StatusCode = statusCode;
            this.Actions = actions;
            AddBody(body);
        }
        private ResponseTemplate() {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }

        public string User { get; private set; }
        public List<ActionTemplate> Actions { get; private set; }
        public string ResponseId { get; private set; }
        public int StatusCode { get; private set; }
        public string Body { get; private set; }

        public dynamic GetBody() {
            return JsonConvert.DeserializeObject<dynamic>(this.Body);
        }

        public void AddBody(dynamic body) {
            this.Body = JsonConvert.SerializeObject(body);
        }

    }
}
