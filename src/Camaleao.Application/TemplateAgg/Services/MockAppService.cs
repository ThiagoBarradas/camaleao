using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using Camaleao.Core.Entities.Request;
using Camaleao.Core.Repository;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Camaleao.Application.TemplateAgg.Services {
    public class MockAppService {

        private readonly ITemplateRepository _templateRepository;
        private readonly IResponseRepository _responseRepository;
        private readonly IContextRepository _contextRepository;

        public MockAppService(ITemplateRepository templateRepository,
                              IResponseRepository responseRepository,
                              IContextRepository contextRepository) {

            this._templateRepository = templateRepository;
            this._responseRepository = responseRepository;
            this._contextRepository = contextRepository;
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

        public void Post(string user, RouteTemplate route, HttpContext context) {

            // buscar o template
            var template = _templateRepository.Get(p => p.User == user &&
                    p.Route.Version == route.Version &&
                    p.Route.Name == route.Name &&
                     p.Route.Method == route.Method).FirstOrDefault();


            //Comparar se o Request recebido satifaz as informações do template
            var postRequestRecived = new PostRequestRecived((PostRequestTemplate)template.Request, context.Request.Body);
            
            //Se o request não for válido eu retorno um badrequest
            if (!postRequestRecived.IsValid())
                return;

            //Pesquiso o contexto

            //Percorro as regras para achar o response



            //Executo as actions globais

            //Executo as actions do response

            //Substituo os valores do response

            //Retorno o response




        }
    }
}
