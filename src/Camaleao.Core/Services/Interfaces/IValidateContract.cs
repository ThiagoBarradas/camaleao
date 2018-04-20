using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IValidate
    {
        IReadOnlyCollection<Notification> ValidateContract();
    }
}
