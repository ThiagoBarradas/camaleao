using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static string ClearNavigateProperties(this string key)
        {
            if (key.ExtractBetween("[", "]") != String.Empty)
            {
                key.ExtractList("[", "]").ForEach(k => key = key.Replace(k, "[0]"));
            }

            return key;
        }

        public static string ExtractBetween(this string content, params string[] delimeters)
        {
            int StartPosition = 0, EndPosition = 0;

            if (content.Contains(delimeters.FirstOrDefault()) && content.Contains(delimeters.LastOrDefault()))
            {
                StartPosition = content.IndexOf(delimeters.FirstOrDefault(), EndPosition) + delimeters.FirstOrDefault().Length;
                EndPosition = content.IndexOf(delimeters.LastOrDefault(), StartPosition);
                return content.Substring(StartPosition, EndPosition - StartPosition);
            }

            return String.Empty;
        }

        public static string Extract(this string content, params string[] delimeters)
        {
            string extracted = String.Empty;
            if ((extracted = content.ExtractBetween(delimeters)) != String.Empty)
                return String.Format(@"{0}{1}{2}", delimeters.FirstOrDefault(), extracted, delimeters.LastOrDefault());
            return extracted;
        }
        public static List<string> ExtractList(this string content, params string[] delimeters)
        {
            var elements = new List<string>();

            string extracted = String.Empty;
            while ((extracted = content.Extract(delimeters)) != String.Empty)
            {
                content = content.Replace(extracted, String.Empty);
                elements.Add(extracted);
            }
            return elements;
        }
    }


}
