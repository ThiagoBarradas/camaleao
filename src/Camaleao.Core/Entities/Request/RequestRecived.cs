namespace Camaleao.Core.Entities.Request {
    public abstract class RequestRecived {
        public abstract bool IsValid();
        public abstract string GetContextIdentifier();
        public abstract RequestTemplate GetRequestTemplate();
    }
}
