using Camaleao.Core.Entities;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IGetMockService
    {
        void StartUp(Template template, string[] queryString);
        IReadOnlyCollection<Notification> ValidateContract();
        IReadOnlyCollection<Notification> ValidateRules();

        void LoadContext();

        ResponseTemplate Response();
    }
}
