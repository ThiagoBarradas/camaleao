using Camaleao.Core.Repository;
using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using Camaleao.Core.Entities;
using Flunt.Notifications;

namespace Camaleao.Core.Services
{
    public class ContextService : Notifiable, IContextService
    {
        readonly IContextRepository contextRepository;

        public ContextService(IContextRepository contextRepository)
        {
            this.contextRepository = contextRepository;
        }

        public void Add(Context context)
        {
            contextRepository.Add(context);
        }

        public Context FirstOrDefault(string contextKey)
        {
            Context context =null;
            context = this.contextRepository.Get(p => p.Id == Guid.Parse(contextKey)).FirstOrDefault();

            if(context==null)
                AddNotification($"Context", $"There isn't registered context with this ID: {contextKey}");

            return context;
        }
        public Context FirstOrDefaultByExternalIdentifier(string externalIdentifier)
        {
            Context context = null;
            context = this.contextRepository.Get(p => p.ExternalIdentifier == externalIdentifier).FirstOrDefault();

            return context;
        }

        public void Update(Context context)
        {
            this.contextRepository.Update(p => p.Id == context.Id, context);
        }

        IReadOnlyCollection<Notification> IContextService.Notifications()
        {
            return Notifications;
        }
    }
}
