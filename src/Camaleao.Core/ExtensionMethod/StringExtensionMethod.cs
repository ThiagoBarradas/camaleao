using Newtonsoft.Json.Linq;
using System;

namespace Camaleao.Core.ExtensionMethod
{
    public static class StringExtensionMethod
    {
        public static Type GetTypeChameleon(this string content)
        {
            string type = null;

            switch (content)
            {
                case "texto":
                    type = "System.String";
                    break;
                case "inteiro":
                    type = "System.Int32";
                    break;
                case "decimal":
                    type = "System.Double";
                    break;
                case "bool":
                    type = "System.Boolean";
                    break;
            }

            return Type.GetType(type, false, true);
        }

        public static Type GetTypeJson(this JToken content)
        {
            string type = null;

            switch (content.Type)
            {
                case JTokenType.String:
                    type = "System.String";
                    break;
                case JTokenType.Integer:
                    type = "System.Int32";
                    break;
                case JTokenType.Float:
                    type = "System.Double";
                    break;
                case JTokenType.Boolean:
                    type = "System.Boolean";
                    break;
            }

            return Type.GetType(type, false, true);
        }

    }
}
