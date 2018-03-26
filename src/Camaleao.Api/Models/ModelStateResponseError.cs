using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Camaleao.Api.Models
{
    public static class ModelStateResponseError
    {
        public static ErrorResponse GetErrorResponse(this ModelStateDictionary modelState)
        {
            return new ErrorResponse()
            {
                Erros = modelState.Values.SelectMany(v => v.Errors).Select(p => p.ErrorMessage.ToString()).ToArray()
            };

        }
    }
}
