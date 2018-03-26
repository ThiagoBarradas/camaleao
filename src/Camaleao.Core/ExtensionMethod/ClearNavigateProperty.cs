using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camaleao.Core.ExtensionMethod
{
    public static class ClearNavigateProperty
    {
        public static string ClearNavigateProperties(this string key)
        {
            if (ExtractBetween(key, "[", "]") != String.Empty)
            {
                ExtractList(key, "[", "]").ForEach(k => key = key.Replace(k, "[0]"));
            }

            return key;
        }

        public static string ExtractBetween(string content, params string[] delimeters)
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

        public static string Extract(string content, params string[] delimeters)
        {
            string extracted = String.Empty;
            if ((extracted = ExtractBetween(content, delimeters)) != String.Empty)
                return String.Format(@"{0}{1}{2}", delimeters.FirstOrDefault(), extracted, delimeters.LastOrDefault());
            return extracted;
        }
        public static List<string> ExtractList(string content, params string[] delimeters)
        {
            var elements = new List<string>();

            string extracted = String.Empty;
            while ((extracted = Extract(content, delimeters)) != String.Empty)
            {
                content = content.Replace(extracted, String.Empty);
                elements.Add(extracted);
            }
            return elements;
        }
    }
}
