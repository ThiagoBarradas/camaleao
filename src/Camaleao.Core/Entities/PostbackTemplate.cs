using Camaleao.Core.SeedWork;
using Camaleao.Core.Services.Interfaces;
using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camaleao.Core.Entities {
    public class PostbackTemplate {

        private PostbackTemplate() {

        }

        public PostbackTemplate(byte delayInSeconds, string responseId, string url, List<ActionTemplate> actions) {
            this.DelayInSeconds = delayInSeconds;
            this.ResponseId = responseId;
            this.Url = url;
            this.Actions = actions;
        }
        public byte DelayInSeconds { get; private set; }
        public string ResponseId { get; private set; }
        public string Url { get; private set; }
        public List<ActionTemplate> Actions { get; private set; }

        public void ExecuteActions(IEngineService engineService) {

            if(this.Actions!=null)
                this.Actions?.ForEach(p => engineService.Execute<string>(p.Execute.ExtractExpressionAction(engineService)));
        }
        public void Send(dynamic body, IEngineService engineService) {

            if (this.DelayInSeconds > 0)
                Thread.Sleep(this.DelayInSeconds * 1000);

            Task.Run(() => {
                RestClient restClient = new RestClient(this.Url.ExtractExpressionResponse(engineService));
                RestRequest restRequest = new RestRequest(Method.POST);

                restRequest.AddParameter("application/json", body, ParameterType.RequestBody);
                restClient.Execute(restRequest);
            });
        

        }
    }
}
