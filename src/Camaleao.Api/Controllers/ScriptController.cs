using Camaleao.Api.Models;
using Camaleao.Core;
using Camaleao.Core.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Camaleao.Api.Controllers
{
    [Route(RouteConfig.Script)]
    [Produces("application/json")]
    public class ScriptController : Controller
    {
        private readonly IScriptRepository _scriptRepository;
        public ScriptController(IScriptRepository scriptRepository)
        {
            _scriptRepository = scriptRepository;
        }

        /// <summary>
        /// Create scripts 
        /// </summary>
        /// <param name="scriptRequest"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Add([FromBody]ScriptRequest scriptRequest)
        {
            try
            {
                ScriptEngine scriptEngine = new ScriptEngine()
                {
                    Id = Guid.NewGuid(),
                    Script = scriptRequest.Script
                };
                _scriptRepository.Add(scriptEngine);

                return new ObjectResult("Script created with success.") { StatusCode = 200 };
            }catch
            {
                return new ObjectResult("Erro on created Script.") { StatusCode = 500 };
            }
        }

    }
}
