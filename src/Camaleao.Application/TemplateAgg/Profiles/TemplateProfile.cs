using AutoMapper;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using System;

namespace Camaleao.Application.TemplateAgg.Profiles {
    public class TemplateProfile:Profile
    {
        public TemplateProfile() {
            CreateMap<VariableModel, Variable>()
                .ForMember(dest => dest.Builded, opt => opt.Ignore());
            CreateMap<ResponseModel, ResponseTemplate>();
            CreateMap<RequestModel, RequestTemplate>()
                .ConstructUsing(GetRequest);
            CreateMap<ContextModel, ContextTemplate>();
            CreateMap<RouteModel, RouteTemplate>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToLower()))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version.ToLower()));
            CreateMap<CreateTemplateRequestModel, Template>()
                .ConstructUsing(src => new Template(Guid.NewGuid()));
 


            CreateMap<PostRequestTemplate, RequestModel>();
            CreateMap<GetRequestTemplate, RequestModel>();
            CreateMap<Template, TemplateResponseModel>();
               // .ForMember(dest => dest.Route, opt => opt.ResolveUsing(CreateRoute));
        }

        private static string CreateRoute(Template template) {
            return $"api/{template.User}/{template.Route.Version}/{template.Route.Name}";
        }

        private static RequestTemplate GetRequest(RequestModel requestModel)
        {
            if (requestModel.Body != null)
                return new PostRequestTemplate(requestModel.Body) { Headers = requestModel.Headers };
            else
                return new GetRequestTemplate() { Headers = requestModel.Headers, QueryString = requestModel.QueryString };
            
        }
    }
}
