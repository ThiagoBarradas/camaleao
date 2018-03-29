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
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore());
            CreateMap<Models.Context, Core.Context>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore());
            CreateMap<TemplateRequest, Template>();
        }
    }
}
