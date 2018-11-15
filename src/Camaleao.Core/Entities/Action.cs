using Flunt.Notifications;

namespace Camaleao.Core.Entities
{
    public class Action: Notifiable
    {
        public string Execute { get; set; }

        public bool UseContext()
        {
            return Execute.Contains(Enuns.VariableTypeEnum.Context);
        }
    }
}
