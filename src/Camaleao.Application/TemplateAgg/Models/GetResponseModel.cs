using Newtonsoft.Json;

namespace Camaleao.Application.TemplateAgg.Models {
    public class GetResponseModel {

        public GetResponseModel(int statusCode) {
            this.StatusCode = statusCode;
        }
        [JsonIgnore]
        public int StatusCode { get; private set; }

        public dynamic Body { get; set; }

    }
}
