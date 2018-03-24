using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.ExtensionMethod
{
    public static class StringExtensionMethod
    {
        public static Type GetTypeChameleon(this string content)
        {
            string type = null;
            
            if(content == "texto")
                type = "System.String";
            else if(content == "inteiro")
                type = "System.Int32";
            else if(content == "decimal")
                type = "System.Double";
            else if(content == "bool")
                type = "System.Boolean";

            return Type.GetType(type, false, true);
        }

        public static Type GetTypeJson(this JToken content)
        {
            string type = null;

            if(content.Type == JTokenType.String)
                type = "System.String";
            else if(content.Type == JTokenType.Integer)
                type = "System.Int32";
            else if(content.Type == JTokenType.Float)
                type = "System.Double";
            else if(content.Type == JTokenType.Boolean)
                type = "System.Boolean";

            return Type.GetType(type, false, true);
        }

    }
}
