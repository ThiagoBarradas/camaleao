using Camaleao.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.SeedWork {
    public static class ExpressionUtility {


        public static List<string> ExtractVariables(string expression) {

            List<string> variables = new List<string>();

            ExtractContext(ref expression, variables);

            return variables;

        }

        private static void ExtractContext(ref string expression, List<string> variables) {

            var variable = getBetween(expression, "_context.{{", "}}");
            if (variable != string.Empty) {
                variables.Add(variable);
                var maskVariable = $"_context.{{{{{variable}}}}}";
                expression = expression.Replace(maskVariable, "");
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


        public static string ExtractExpression(this string expression, IEngineService engineService) {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
           {
                new ExtractContextExpression(engineService,false, ScopeExpression.NoScope,false),
                new ExtractContextComplexElementExpression(engineService,false,ScopeExpression.NoScope,false),
                new ExtractElementExpression(engineService,false,ScopeExpression.NoScope,false),
                new ExtractComplextElementExpression(engineService,false,ScopeExpression.NoScope,false)
            }, expression);
        }

        public static string ExtractExpressionAction(this string expression, IEngineService engineService) {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
           {
                new ExtractContextExpression(engineService,false, ScopeExpression.NoScope,true),
                new ExtractContextComplexElementExpression(engineService,false,ScopeExpression.NoScope,true),
                new ExtractElementExpression(engineService,true,ScopeExpression.NoScope,true)
            }, expression);
        }

        public static string ExtractExpressionResponse(this string expression, IEngineService engineService) {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
           {
                new ExtractContextExpression(engineService,true,ScopeExpression.Response,false),
                new ExtractContextComplexElementExpression(engineService,true,ScopeExpression.Response,false),
                new ExtractElementExpression(engineService,true,ScopeExpression.Response,false),
                new ExtractContextComplexElementExpression(engineService,true,ScopeExpression.Response,false)
            }, expression);
        }


    }
}
