namespace Camaleao.Application.TemplateAgg.Models {
    public class CreateResponseTemplateResponseModel : BaseResponseModel {

        public CreateResponseTemplateResponseModel(int statusCode):base(statusCode) {
        }
        public ResponseModel ResponseModel { get; set; }
    }
}
