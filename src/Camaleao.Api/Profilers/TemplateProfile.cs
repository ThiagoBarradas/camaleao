using AutoMapper;
using Camaleao.Api.Models;
using Camaleao.Core;
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
            CreateMap<Models.Response, Core.Response>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
                .ForPath(dest => dest.Callback, opt => opt.MapFrom(src => src.Callback.ResponseId))
                .ForPath(dest => dest.Variables, opt => opt.MapFrom(src => src.Callback.Variables));
            CreateMap<Models.Callback, Core.Callback>()
                .ForAllMembers(dest => dest.Ignore());
            CreateMap<TemplateRequest, Template>();
        }
    }
}
