﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IEngineService
    {
        T Execute<T>(string expression);
        void LoadRequest(JObject request, string variavel);
    }
}
