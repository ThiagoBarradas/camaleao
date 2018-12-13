namespace Camaleao.Application.TemplateAgg.Models {
    public class CreateOrUpdateResponseTemplateResponseModel : BaseResponseModel {

        public CreateOrUpdateResponseTemplateResponseModel(int statusCode):base(statusCode) {
        }
        public ResponseModel ResponseModel { get; set; }
    }
}
