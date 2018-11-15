using Camaleao.Core.Entities;
using Flunt.Notifications;
using System.Collections.Generic;

namespace Camaleao.Core.Validates {
    public interface ICreateTemplateValidate {
         
        bool Validate(Template template);

        IReadOnlyCollection<Notification> GetNotifications();
    }
}
