using Camaleao.Core.Enuns;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Entities {
    public class GetRequestTemplate : RequestTemplate {
        public string QueryString { get; set; }

        Dictionary<int, string> queryStringidentifiers;
        public Dictionary<int, string> GetIdentifierFromQueryString() {

            if (queryStringidentifiers == null)
                queryStringidentifiers = new Dictionary<int, string>();
            else
                return queryStringidentifiers;

            var queryStringArray = QueryString.Split(new Char[] { '/', '?','&','=' });
            for (int i = 0; i < queryStringArray.Length; i++) {


                if ((queryStringArray[i].Contains("{{") && queryStringArray[i].Contains("}}")) 
                    || queryStringArray[i].Contains(VariableTypeEnum.Context) 
                    || queryStringArray[i].Contains(VariableTypeEnum.ExternalContext))
                    queryStringidentifiers.Add(i, queryStringArray[i]);
            }

            return queryStringidentifiers;
        }

        public override bool IsValid() {
            return true;
        }
    }
}
