using Flunt.Notifications;
using System.Linq;

namespace Camaleao.Core.Entities {
    public class RouteTemplate: Notifiable {

        public RouteTemplate(string version, string name, string method) {
            this.Version = version.ToLower();
            this.Name = name.ToLower();
            this.Method = method.ToLower();
        }

        public RouteTemplate()
        {

        }
        public string Version { get; private set; }
        public string Name { get; private set; }
        public string Method { get; private set; }


        public static RouteTemplate Create(string name, string version, string method) {
            return new RouteTemplate( version,name, method);
        }
        public bool IsValid() {
            if (!(new string[] { "get", "post" }).Contains(this.Method)) {
                AddNotification("Method", "Route method inválid.");
                return false;
            }
            return true;
        }
    }
}
