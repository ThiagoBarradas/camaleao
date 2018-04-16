using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camaleao.Api.Profilers
{
    public class TemplateProfile:Profile
    {
        public TemplateProfile()
        {
            CreateMap<VariableModel, Variable>()
                .ForMember(dest => dest.Builded, opt => opt.Ignore());
            CreateMap<ResponseModel, ResponseTemplate>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore());
            CreateMap<ContextModel, ContextTemplate>();
            CreateMap<RouteModel, Route>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.ToLower()))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version.ToLower()));
            CreateMap<TemplateRequestModel, Template>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Template, TemplateResponseModel>();
        }
    }
}
