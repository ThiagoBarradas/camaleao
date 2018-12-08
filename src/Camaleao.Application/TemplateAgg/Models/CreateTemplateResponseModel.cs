namespace Camaleao.Application.TemplateAgg.Models {
    public class CreateTemplateResponseModel: BaseResponseModel {

        public CreateTemplateResponseModel( int statusCode):base(statusCode) {
        }
        public string Route { get; set; }
        public string Token { get; set; }
        public string Method { get; set; }

    } 
}
