namespace Camaleao.Application.TemplateAgg.Models {
    public class MockResponseModel {


        public MockResponseModel(int statusCode, string body) {
            this.StatusCode = statusCode;
            this.Body = body;
        }
        public int StatusCode { get; private set; }
        public string Body { get; private set; }
    }
}
