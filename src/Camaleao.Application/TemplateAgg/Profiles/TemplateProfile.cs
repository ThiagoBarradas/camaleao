using AutoMapper;
using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using System;

namespace Camaleao.Application.TemplateAgg.Profiles {
    public class TemplateProfile : Profile {
        public TemplateProfile() {
            MapRequest();

            CreateMap<Template, TemplateResponseModel>();

            CreateMap<PostRequestTemplate, RequestModel>();

            CreateMap<GetRequestTemplate, RequestModel>();

            CreateMap<ResponseTemplate, ResponseModel>()
                .ForMember(dest => dest.ResponseId, opt => opt.MapFrom(src => src.ResponseId.ToLower()))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
                .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions))
                .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.GetBodyInJson()));
        }

        private void MapRequest() {
            CreateMap<ActionModel, ActionTemplate>()
                .ForMember(dest => dest.Execute, opt => opt.MapFrom(src => src.Execute));

            CreateMap<ResponseModel, ResponseTemplate>()
                .ForMember(dest => dest.ResponseId, opt => opt.MapFrom(src => src.ResponseId.ToLower()))
                .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
                .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions))
                .AfterMap((src, dest) => dest.AddBody(src.Body));

            CreateMap<Postback, PostbackTemplate>()
                .ForMember(dest => dest.ResponseId, opt => opt.MapFrom(src => src.ResponseId.ToLower()))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.DelayInSeconds, opt => opt.MapFrom(src => src.DelayInSeconds));

            CreateMap<RuleModel, RuleTemplate>()
                .ForMember(dest => dest.ResponseId, opt => opt.MapFrom(src => src.ResponseId.ToLower()))
                .ForMember(dest => dest.Expression, opt => opt.MapFrom(src => src.Expression))
                .ForMember(dest => dest.Postback, opt => opt.MapFrom(src => src.Postback));

            CreateMap<VariableModel, Variable>()
                .ForMember(dest => dest.Initialize, opt => opt.MapFrom(src => src.Initialize))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .AfterMap((src, dest) => dest.BuildVariable());

            CreateMap<RequestModel, RequestTemplate>()
                .ConstructUsing(src => GetRequest(src));

            CreateMap<ContextModel, ContextTemplate>();


            
            CreateMap<RouteModel, RouteTemplate>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToLower()))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version.ToLower()));

            CreateMap<CreateTemplateRequestModel, Template>()
                .ForMember(dest => dest.Actions, opt => opt.MapFrom(src => src.Actions))
                .ForMember(dest => dest.Context, opt => opt.MapFrom(src => src.Context))
                .ForMember(dest => dest.Request, opt => opt.MapFrom(src => src.Request))
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules))
                .ForMember(dest => dest.Invalid, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ResponsesId, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore());

            CreateMap<UpdateTemplateRequestModel, Template>()
             .ConstructUsing(src => new Template(src.Token));

        }

        private static string CreateRoute(Template template) {
            return $"api/{template.User}/{template.Route.Version}/{template.Route.Name}";
        }

        private static RequestTemplate GetRequest(RequestModel requestModel) {
            if (requestModel.Body != null)
                return new PostRequestTemplate(requestModel.Body) { Headers = requestModel.Headers };
            else
                return new GetRequestTemplate() { Headers = requestModel.Headers, QueryString = requestModel.QueryString };

        }
    }
}
