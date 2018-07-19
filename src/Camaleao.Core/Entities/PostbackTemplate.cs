using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camaleao.Core.Entities {
    public class PostbackTemplate {
        public byte DelayInSeconds { get; set; }
        public string ResponseId { get; set; }
        public string Url { get; set; }


        public async Task Send(dynamic body) {

            if (this.DelayInSeconds > 0)
                Thread.Sleep(this.DelayInSeconds * 1000);

            RestClient restClient = new RestClient(this.Url);
            RestRequest restRequest = new RestRequest(Method.POST);

            restRequest.AddParameter("application/json",body,ParameterType.RequestBody);

            restClient.Execute(restRequest);

        }
    }
}
