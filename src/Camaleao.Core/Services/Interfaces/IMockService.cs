using Camaleao.Core.Entities;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IMockService
    {
        void InitializeMock(Template template, JObject request);
        IReadOnlyCollection<Notification> ValidateContract();
        IReadOnlyCollection<Notification> ValidateRules();
        ResponseTemplate Response();
    }
}
