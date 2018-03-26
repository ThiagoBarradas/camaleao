using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(string serverPath)
        {
            _serverPath = serverPath;
        }

        private readonly string _serverPath = string.Empty;
        public string ServerPath => _serverPath;
    }
}
