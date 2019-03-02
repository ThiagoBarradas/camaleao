using Camaleao.Core.Entities.Request;
using Camaleao.Core.Enuns;
using Camaleao.Core.SeedWork;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Camaleao.Core.Entities {
    public class ResponseTemplate : Entity {

        private ResponseTemplate() {

        }

        public ResponseTemplate(ObjectId Id, string user, string responseId, int statusCode, dynamic body, List<ActionTemplate> actions) {
            this.User = user;
            this.ResponseId = responseId.ToLower();
            this.StatusCode = statusCode;
            this.Actions = actions;
            AddBody(body);
        }


        public string User { get; private set; }
        public List<ActionTemplate> Actions { get; private set; }
        public string ResponseId { get; private set; }
        public int StatusCode { get; private set; }
        public string Body { get; private set; }

        public dynamic GetBodyInJson() {
            return JsonConvert.DeserializeObject<dynamic>(this.Body);
        }

        public void AddBody(dynamic body) {
            this.Body = JsonConvert.SerializeObject(body);
        }
        public void AddUser(string user) {
            this.User = user;
        }
        public void UpdateId(Guid id) {
            this.Id = id;
        }

 
        public void ProcessBody(IEngineService engineService, RequestRecived requestRecived) {

            if (this.Actions != null)
                this.Actions.ForEach(p => engineService.Execute<string>(p.Execute.ExtractExpressionAction(engineService)));

            this.Body = this.Body.ExtractExpressionResponse(engineService) ?? string.Empty;

            this.Body = this.Body.Replace(VariableTypeEnum.ExternalContext, requestRecived.GetContextIdentifier())
                          .Replace(VariableTypeEnum.Context, requestRecived.GetContextIdentifier());
        }
        

        public override bool IsValid() {
            return true;
        }
    }
}
