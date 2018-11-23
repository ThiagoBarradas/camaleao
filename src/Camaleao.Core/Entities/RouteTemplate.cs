using Flunt.Notifications;
using System.Linq;

namespace Camaleao.Core.Entities {
    public class RouteTemplate: Notifiable {

        #region Constructores
        public RouteTemplate(string version, string name, string method) {
            this.Version = version.ToLower();
            this.Name = name.ToLower();
            this.Method = method.ToLower();
        }
        private RouteTemplate()
        {

        }
        #endregion

        #region Properties
        public string Version { get; private set; }
        public string Name { get; private set; }
        public string Method { get; private set; }
        #endregion

        #region Methods
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

        #endregion
    }
}
