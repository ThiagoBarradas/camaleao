using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Services.Interfaces;

namespace Camaleao.Core
{
    public class ExtractExpression
    {

        public string Extract(IList<ExtractProperties> extractProperties, string expression)
        {
            foreach(var item in extractProperties)
            {
                expression = item.Extract(expression);
            }

            return expression;
        }
    }

    public interface ExtractProperties
    {
        string Extract(string expression);
    }

    public abstract class AExtractProperties
    {

        private readonly IEngineService _engine;
        private readonly bool execEngine;
        private readonly ScopeExpression scope;

        public AExtractProperties(IEngineService engine, bool execEngine, ScopeExpression scope)
        {
            this._engine = engine;
            this.execEngine = execEngine;
            this.scope = scope;
        }
        protected string ExtractProperties(string expression, string nameFunction = "GetElement", params string[] delimiters)
        {
            var properties = expression.ExtractList(delimiters);
            properties.ForEach(propertie =>
            {
                var content = MapperFunction(nameFunction, propertie);

                if(execEngine)
                    expression = expression.Replace(String.Format(StyleStringFormat(_engine.VariableType(content), scope.ToString(), nameFunction), propertie), _engine.Execute<dynamic>(content));
                else
                    expression = expression.Replace(String.Format(StyleStringFormat(_engine.VariableType(content), scope.ToString(), nameFunction), propertie), content);
            });

            return expression;
        }

        private string StyleStringFormat(string variableType, string scope, string nameFunction)
        {
            if(scope == "Response" && (variableType == "object" || variableType == "number" || nameFunction == "GetContextComplexElement"))
                return @"""{0}""";

            return @"{0}";
        }
        private string MapperFunction(params string[] parameters)
        {
            switch(parameters.FirstOrDefault())
            {
                case "Contains":
                case "NotContains":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween(Delimiters.ElementRequest())}', {parameters[2]})";
                case "ExistPath":
                case "NotExistPath":
                case "GetElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween(Delimiters.ElementRequest())}')";
                case "Context":
                    return $"{parameters[1].ExtractBetween(Delimiters.ContextVariable())}";
                case "ContextComplex":
                    return $"{parameters[1].ExtractBetween(Delimiters.ContextComplexElement())}";
                case "GetComplexElement":
                    return $"{parameters[0]}('{parameters[1].ExtractBetween(Delimiters.ComplexElement())}')";
                case "GetContextComplexElement":
                    return $"JSON.stringify({parameters[1].ExtractBetween(Delimiters.ContextComplexElement())})";
                default:
                    return $"{parameters[0]}({string.Join(',', parameters.Skip(1).ToArray())})";
            }
        }
    }

    public class ExtractContextExpression :AExtractProperties, ExtractProperties
    {
        public ExtractContextExpression(IEngineService engine, bool execEngine, ScopeExpression scope) : base(engine,execEngine,scope)
        {
        }
        public string Extract(string expression)
        {
            expression = ExtractProperties(expression, "Context", delimiters: Delimiters.ContextVariable());

            return expression;
        }
    }

    public class ExtractContextComplexElementExpression :AExtractProperties, ExtractProperties
    {
        public ExtractContextComplexElementExpression(IEngineService engine, bool execEngine, ScopeExpression scope) : base(engine, execEngine, scope)
        {
        }

        public string Extract(string expression)
        {
            expression = ExtractProperties(expression, "GetContextComplexElement", delimiters: Delimiters.ContextComplexElement());
            return expression;
        }
    }

    public class ExtractElementExpression :AExtractProperties, ExtractProperties
    {
        public ExtractElementExpression(IEngineService engine, bool execEngine, ScopeExpression scope) : base(engine, execEngine, scope)
        {
        }

        public string Extract(string expression)
        {
            expression = ExtractProperties(expression,  delimiters: Delimiters.ElementRequest());
            return expression;
        }
    }

    public class ExtractComplextElementExpression :AExtractProperties, ExtractProperties
    {
        public ExtractComplextElementExpression(IEngineService engine, bool execEngine, ScopeExpression scope) : base(engine,execEngine,scope)
        {
        }

        public string Extract(string expression)
        {
            expression = ExtractProperties(expression, "GetComplexElement", Delimiters.ComplexElement());
            return expression;
        }
    }

    public enum ScopeExpression
    {
        NoScope,
        Response
    }
}
