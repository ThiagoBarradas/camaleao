using Flunt.Notifications;

namespace Camaleao.Core.Entities
{
    public class ActionTemplate: Notifiable
    {
        private ActionTemplate() {

        }
        public ActionTemplate(string execute) {
            this.Execute = execute;
        }
        public string Execute { get; private set; }

        public bool UseContext()
        {
            return Execute.Contains(Enuns.VariableTypeEnum.Context);
        }
    }
}
