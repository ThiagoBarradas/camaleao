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
            CreateMap<ResponseModel, ResponseTemplate>()
                .ForMember(dest => dest.TemplateId, opt => opt.Ignore());
            CreateMap<ContextModel, ContextTemplate>();
            CreateMap<TemplateRequestModel, Template>();
        }
    }
}
