using Flunt.Notifications;
using MongoDB.Bson;

namespace Camaleao.Core.Entities {
    public abstract class Entity: Notifiable {

        public Entity() {
            this.Id = new ObjectId();
        }
        public ObjectId Id { get; protected set; }

        public abstract bool IsValid();
    }
}
