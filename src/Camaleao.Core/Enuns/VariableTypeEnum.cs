using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.Enuns
{
    public class VariableTypeEnum
    {
        public const string Integer ="integer";
        public const string Text = "text";
        public const string Boolean = "bool";
        public const string Context = "_context";
        public const string ExternalContext = "_context.external";


        public static string[] GetValues()
        {
            var values = new string[] {
                Integer,
                Text,
                Boolean,
                Context,
                ExternalContext
            };
            return values;
        }

        public static string GetMockValueByType(string variableType)
        {
            switch (variableType)
            {
                case Integer:
                    return "0";
                case Boolean:
                    return "true";
                case Text:
                    return "'abc'";
                default:
                    return "";
            }
        }

    }
}
