using Flunt.Notifications;

namespace Camaleao.Core.Entities
{
    public class ActionTemplate: Notifiable
    {
        public string Execute { get; set; }

        public bool UseContext()
        {
            return Execute.Contains(Enuns.VariableTypeEnum.Context);
        }
    }
}
