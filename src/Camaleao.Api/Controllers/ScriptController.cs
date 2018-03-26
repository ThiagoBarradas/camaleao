﻿using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camaleao.Api.Controllers
{
    [Route("api/script")]
    public class ScriptController : Controller
    {
        private readonly IScriptRepository _scriptRepository;
        public ScriptController(IScriptRepository scriptRepository)
        {
            _scriptRepository = scriptRepository;
        }

        [HttpPost()]
        public IActionResult Add([FromBody]ScriptRequest scriptRequest)
        {
            ScriptEngine scriptEngine = new ScriptEngine()
            {
                Id = Guid.NewGuid(),
                Script = scriptRequest.Script
            };
            _scriptRepository.Add(scriptEngine);

            return new ObjectResult("") { StatusCode = 200 };
        }

    }
}
