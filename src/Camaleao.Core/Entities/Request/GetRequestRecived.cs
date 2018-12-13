using Camaleao.Core.Enuns;
using Flunt.Notifications;
using System.Linq;

namespace Camaleao.Core.Entities.Request {
    public class GetRequestRecived : RequestRecived {

        public GetRequestRecived(GetRequestTemplate requestTemplate, string[] queryString) {
            this.RequestTemplate = requestTemplate;
            this.QueryString = queryString;
        }

        public GetRequestTemplate RequestTemplate { get; private set; }
        private string[] QueryString;

        public override bool IsValid() {
         
            foreach (var identifier in this.RequestTemplate.GetIdentifierFromQueryString()){
                if((identifier.Position<QueryString.Length && 
                    string.IsNullOrWhiteSpace(QueryString[identifier.Position]) == false)==false) {
                    return false;
                }
            }
            return true;
        }

        public override string GetContextIdentifier() {

            var item = RequestTemplate.GetIdentifierFromQueryString().FirstOrDefault(p => p.Type == VariableTypeEnum.Context 
            || p.Type == VariableTypeEnum.ExternalContext);

            if (item != null)
                return QueryString[item.Position];

            return string.Empty;
           
        }

        public override RequestTemplate GetRequestTemplate() {
            return this.RequestTemplate;
        }
    }
}
