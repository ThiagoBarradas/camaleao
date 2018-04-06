using Camaleao.Core.Entities;
using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Camaleao.Core.Services.Interfaces
{
    public interface IContextService
    {
        Context FirstOrDefault(string contextKey);
        Context FirstOrDefaultByExternalIdentifier(string externalIdentifier);
        void Add(Context context);
        void Update(Context context);
        IReadOnlyCollection<Notification> Notifications();
    }
}
