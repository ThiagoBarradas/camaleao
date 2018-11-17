using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.SeedWork {
    public class ExpressionUtility {

        public static List<string> ExtractVariables(string expression) {

            List<string> variables = new List<string>();

            ExtractContext(ref expression, variables);

            return variables;

        }

        private static  void ExtractContext(ref string expression, List<string> variables) {
       
            var variable = getBetween(expression, "_context.{{", "}}");
            if (variable != string.Empty) {
                variables.Add(variable);
                var maskVariable = $"_context.{{{{{variable}}}}}";
                expression = expression.Replace(maskVariable,"");
                ExtractContext(ref expression, variables);
            }

        }

        private static string getBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd)) {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else {
                return string.Empty;
            }
        }
    }
}
