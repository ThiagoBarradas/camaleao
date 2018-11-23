using RestSharp;
using System.Threading;
using System.Threading.Tasks;

namespace Camaleao.Core.Entities {
    public class PostbackTemplate {

        private PostbackTemplate() {

        }

        public PostbackTemplate(byte delayInSeconds, string responseId, string url) {
            this.DelayInSeconds = delayInSeconds;
            this.ResponseId = responseId;
            this.Url = url;
        }
        public byte DelayInSeconds { get; set; }
        public string ResponseId { get; set; }
        public string Url { get; set; }


        public async Task Send(dynamic body, string url) {
      
            if (this.DelayInSeconds > 0)
                Thread.Sleep(this.DelayInSeconds * 1000);

            RestClient restClient = new RestClient(url);
            RestRequest restRequest = new RestRequest(Method.POST);

            restRequest.AddParameter("application/json",body,ParameterType.RequestBody);

            restClient.Execute(restRequest);

        }
    }
}
