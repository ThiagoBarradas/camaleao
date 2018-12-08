using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Camaleao.Application.TemplateAgg.Models {
    public abstract class BaseResponseModel {

        public BaseResponseModel(int statusCode) {
            this.StatusCode = statusCode;
        }
        public List<string> Errors { get; set; }
        [JsonIgnore]
        public int StatusCode { get; private set; }
    }

    public static class BaseResponseModelExtension {

        public static T AddError<T>(this BaseResponseModel baseResponseModel, string error) {
            if (baseResponseModel.Errors == null)
                baseResponseModel.Errors = new List<string>();
            baseResponseModel.Errors.Add(error);

            return (T)Convert.ChangeType(baseResponseModel, typeof(T)); 
        }

        public static T AddErros<T>(this BaseResponseModel baseResponseModel, List<string> errors) {
            baseResponseModel.Errors = errors;
            return (T)Convert.ChangeType(baseResponseModel, typeof(T)); ;
        }
    }
}
