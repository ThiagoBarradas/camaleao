using Camaleao.Application.TemplateAgg.Models;
using Camaleao.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Application.TemplateAgg.Services {
    public interface IMockAppService {
        MockResponseModel Execute(MockRequestModel mockRequestModel);
    }
}
