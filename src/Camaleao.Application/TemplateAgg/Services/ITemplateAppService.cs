using Camaleao.Application.TemplateAgg.Models;
using System.Collections.Generic;

namespace Camaleao.Application.TemplateAgg.Services
{
    /// <summary>
    /// ITemplateAppService
    /// </summary>
    public interface ITemplateAppService
    {
        CreateTemplateResponseModel Create(string user, CreateTemplateRequestModel createTemplateRequestModel);
        GetRequestTemplateResponseModel Get(GetRequestTemplateRequestModel request);
        GetRequestTemplateResponseModel Generate(string  user, dynamic body);
        CreateTemplateResponseModel Update(string user, UpdateTemplateRequestModel updateTemplateRequestModel);
        CreateOrUpdateResponseTemplateResponseModel CreateResponse(string user, ResponseTemplateResquestModel responseModel);
        CreateOrUpdateResponseTemplateResponseModel UpdateResponse(string user, ResponseTemplateResquestModel responseModel);
        List<ResponseModel> GetResponsesByUser(string user)
    }
}
