using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Repository;
using System.Linq;

namespace Camaleao.Application.TemplateAgg.Services {
    public class MockAppService {

        private readonly ITemplateRepository _templateRepository;
        private readonly IResponseRepository _responseRepository;

        public MockAppService(ITemplateRepository templateRepository,
                              IResponseRepository responseRepository) {

            this._templateRepository = templateRepository;
            this._responseRepository = responseRepository;
        }

        public GetResponseModel Get(GetRequestModel getRequestModel) {


            if (getRequestModel.Route == null) {
                return new GetResponseModel(404) {
                    Body = "Page Not Found!"
                };
            }
            var template = _templateRepository.Get(p => p.Route.Name.ToLower() == getRequestModel.Route.Name.ToLower()
                                       && p.User.ToLower() == getRequestModel.User.ToLower()
                                       && p.Route.Method.ToLower() == getRequestModel.Route.Method.ToLower()
                                       && p.Route.Version.ToLower() == getRequestModel.Route.Version.ToLower()).FirstOrDefault();

            if (template == null) {
                return new GetResponseModel(404) {
                    Body = "Page Not Found!"
                };
            }


            return null;

        }

        public void Post() {

        }
    }
}
