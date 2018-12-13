using Camaleao.Core.Enuns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Camaleao.Core.Entities {
    public class GetRequestTemplate : RequestTemplate {
        public string QueryString { get; set; }

        List<QueryStringIdentifier> queryStringIdentifier;
        public List<QueryStringIdentifier> GetIdentifierFromQueryString() {

            if (queryStringIdentifier == null)
                queryStringIdentifier = new List<QueryStringIdentifier>();
            else
                return queryStringIdentifier;

            var queryStringArray = QueryString.Split(new Char[] { '/', '?', '&', '=' });
            for (int i = 0; i < queryStringArray.Length; i++) {



                if (queryStringArray[i].Contains("{{") && queryStringArray[i].Contains("}}")) {
                    queryStringIdentifier.Add(new QueryStringIdentifier((byte)i, VariableTypeEnum.Text, queryStringArray[i]));
                }
                else if (queryStringArray[i].Contains(VariableTypeEnum.Context)) {
                    queryStringIdentifier.Add(new QueryStringIdentifier((byte)i, VariableTypeEnum.Context, queryStringArray[i]));
                }
                else if (queryStringArray[i].Contains(VariableTypeEnum.ExternalContext)) {
                    queryStringIdentifier.Add(new QueryStringIdentifier((byte)i, VariableTypeEnum.ExternalContext, queryStringArray[i]));
                }
            }

            return queryStringIdentifier;
        }

        public override bool IsValid() {
            return true;
        }

        public  override bool UseContext() {
            return this.GetIdentifierFromQueryString()
                .FirstOrDefault(p => p.Type == VariableTypeEnum.Context) != null;
        }

        public override bool UseExternalContext() {
            return this.GetIdentifierFromQueryString()
                .FirstOrDefault(p => p.Type == VariableTypeEnum.ExternalContext) != null;
        }
    }

    public class QueryStringIdentifier {

        public QueryStringIdentifier(byte position, string type, string value = "") {
            this.Position = position;
            this.Type = type;
            this.Value = value;
        }
        public byte Position { get; private set; }
        public string Type { get; private set; }
        public string Value { get; private set; }
    }
}
