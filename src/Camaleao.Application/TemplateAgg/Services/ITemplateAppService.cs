using Camaleao.Application.TemplateAgg.Models;

namespace Camaleao.Application.TemplateAgg.Services
{
    public interface ITemplateAppService
    {
        CreateTemplateResponseModel Create(string user, CreateTemplateRequestModel createTemplateRequestModel);
        GetRequestTemplateResponseModel Get(GetRequestTemplateRequestModel request);
    }
}
