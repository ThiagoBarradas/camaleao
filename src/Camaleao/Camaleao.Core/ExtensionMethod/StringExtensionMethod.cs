using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.ExtensionMethod
{
    public static class StringExtensionMethod
    {
        public static Type GetTypeChameleon(this string content)
        {
            var type = "System.String";

            if(content == "texto")
                type = "System.String";
            else if(content == "inteiro")
                type = "System.Int32";
            else if(content == "decimal")
                type = "System.Double";

            return Type.GetType(type, false, true);
        }

        public static string Replace(this string content, Dictionary<string, object> properties, bool mapperType)
        {
            var startString = "{{";
            var endString = "}}";

            int Start = 0, End = 0;

            while(content.Contains(startString) && content.Contains(endString))
            {
                Start = content.IndexOf(startString, End) + startString.Length;
                End = content.IndexOf(endString, Start);
                var texto = content.Substring(Start, End - Start);
                string valor = mapperType ? MapperType(properties[texto]) : Convert.ToString(properties[texto]);
                content = content.Replace(startString + texto + endString, valor);
            }

            content = content.Replace("'", "\"");
            return content;
        }

        private static string MapperType(object conteudo)
        {
            if(conteudo.GetType() == typeof(string))
                return String.Format(@"""{0}""", conteudo);

            if(conteudo.GetType() == typeof(double))
                return String.Format(new System.Globalization.CultureInfo("en-US"), "{0:0.00}", conteudo);

            return Convert.ToString(conteudo);
        }
    }
}
