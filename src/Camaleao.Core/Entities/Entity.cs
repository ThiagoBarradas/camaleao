using Flunt.Notifications;
using System;

namespace Camaleao.Core.Entities {
    public abstract class Entity: Notifiable {

        public Entity() {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; protected set; }

        public abstract bool IsValid();
    }
}
