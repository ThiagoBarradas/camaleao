using Newtonsoft.Json;
using System.Collections.Generic;

namespace Camaleao.Application.TemplateAgg.Models {
    public class CreateTemplateResponseModel {

        public CreateTemplateResponseModel(int statusCode) {
            this.StatusCode = statusCode;
        }
        public string Route { get; set; }
        public string Token { get; set; }
        public string Method { get; set; }
        public List<string> Errors { get; set; }
        [JsonIgnore]
        public int StatusCode { get; private set; }
    }

    public static class CreateTemplateResponseModelExtension {


        public static CreateTemplateResponseModel AddError(this CreateTemplateResponseModel createTemplateResponseModel, string error) {
            if (createTemplateResponseModel.Errors == null)
                createTemplateResponseModel.Errors = new List<string>();
            createTemplateResponseModel.Errors.Add(error);
            return createTemplateResponseModel;
        }

        public static CreateTemplateResponseModel AddErros(this CreateTemplateResponseModel createTemplateResponseModel, List<string> errors) {
            createTemplateResponseModel.Errors = errors;
            return createTemplateResponseModel;
        }
    }
}
