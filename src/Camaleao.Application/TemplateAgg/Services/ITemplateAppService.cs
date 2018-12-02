using Camaleao.Application.TemplateAgg.Models;

namespace Camaleao.Application.TemplateAgg.Services
{
    public interface ITemplateAppService
    {
        CreateTemplateResponseModel Create(string user, CreateTemplateRequestModel createTemplateRequestModel);
        GetRequestTemplateResponseModel Get(GetRequestTemplateRequestModel request);
        GetRequestTemplateResponseModel Generate(string  user, dynamic body);
        CreateTemplateResponseModel Update(string user, UpdateTemplateRequestModel updateTemplateRequestModel);
    }
}
