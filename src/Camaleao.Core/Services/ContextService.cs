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
            this.contextRepository.Add( context);
        }

        public Context GetContext(string contextKey)
        {
            Context context =null;
            context = this.contextRepository.Get(p => p.Id == Guid.Parse(contextKey)).Result.FirstOrDefault();

            if(context==null)
                AddNotification($"Context", $"There isn't registered context with this ID: {contextKey}");

            return context;
        }

        public void Update(Context context)
        {
            this.contextRepository.Update(p => p.Id == context.Id, context);
        }
    }
}
