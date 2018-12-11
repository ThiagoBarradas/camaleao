using System;
using System.Collections.Generic;
using System.Linq;
using Camaleao.Core.Services.Interfaces;

namespace Camaleao.Core {
    public class ExtractExpression {

        public string Extract(IList<ExtractProperties> extractProperties, string expression) {
            foreach (var item in extractProperties) {
                expression = item.Extract(expression);
            }

            return expression;
        }
    }

    public interface ExtractProperties {
        string Extract(string expression);
    }

    public abstract class AExtractProperties {

        private readonly IEngineService _engine;
        private readonly bool execEngine;
        private readonly ScopeExpression scope;
        private readonly bool persitElement;

        public AExtractProperties(IEngineService engine, bool execEngine, ScopeExpression scope, bool persitElement) {
            this._engine = engine;
            this.execEngine = execEngine;
            this.scope = scope;
            this.persitElement = persitElement;
        }
        protected string ExtractProperties(string expression, string nameFunction = "GetElement", params string[] delimiters) {
            var properties = expression.ExtractList(delimiters);
            properties.ForEach(propertie => {
                var content = MapperFunction(nameFunction, propertie);

                if (execEngine) {
                    string variableType = _engine.VariableType(content);
                    string styleStringFormat = StyleStringFormat(variableType, scope.ToString(), nameFunction);
                    string stringFormatted = String.Format(styleStringFormat, propertie);
                    string replacedValue = _engine.Execute<dynamic>(content);
                    replacedValue = (variableType == "boolean" || variableType == "bool" || replacedValue=="True") ? replacedValue.ToLower() : replacedValue;
                    expression = expression.Replace(stringFormatted, variableType == "string" && persitElement ? $"'{replacedValue}'" : replacedValue);
                }
                else {
                    string variableType = _engine.VariableType(content);
                    string styleStringFormat = StyleStringFormat(variableType, scope.ToString(), nameFunction);
                    string stringFormatted = String.Format(styleStringFormat, propertie);                 
                    expression = expression.Replace(stringFormatted, content);
                }

            });

            return expression;
        }

        private string StyleStringFormat(string variableType, string scope, string nameFunction) {
            if (scope == "Response" && (variableType == "object" || nameFunction == "GetContextComplexElement"))
                return @"""{0}""";

            return @"{0}";
        }
        private string MapperFunction(params string[] parameters) {
            switch (parameters.FirstOrDefault()) {
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

    public class ExtractContextExpression : AExtractProperties, ExtractProperties {
        public ExtractContextExpression(IEngineService engine, bool execEngine, ScopeExpression scope, bool persitElement) : base(engine, execEngine, scope, persitElement) {
        }
        public string Extract(string expression) {
            expression = ExtractProperties(expression, "Context", delimiters: Delimiters.ContextVariable());

            return expression;
        }
    }

    public class ExtractContextComplexElementExpression : AExtractProperties, ExtractProperties {
        public ExtractContextComplexElementExpression(IEngineService engine, bool execEngine, ScopeExpression scope, bool persitElement) : base(engine, execEngine, scope, persitElement) {
        }

        public string Extract(string expression) {
            expression = ExtractProperties(expression, "GetContextComplexElement", delimiters: Delimiters.ContextComplexElement());
            return expression;
        }
    }

    public class ExtractElementExpression : AExtractProperties, ExtractProperties {
        public ExtractElementExpression(IEngineService engine, bool execEngine, ScopeExpression scope, bool persitElement) : base(engine, execEngine, scope, persitElement) {
        }

        public string Extract(string expression) {
            expression = ExtractProperties(expression, delimiters: Delimiters.ElementRequest());
            return expression;
        }
    }

    public class ExtractComplextElementExpression : AExtractProperties, ExtractProperties {
        public ExtractComplextElementExpression(IEngineService engine, bool execEngine, ScopeExpression scope, bool persitElement) : base(engine, execEngine, scope, persitElement) {
        }

        public string Extract(string expression) {
            expression = ExtractProperties(expression, "GetComplexElement", Delimiters.ComplexElement());
            return expression;
        }
    }

    public enum ScopeExpression {
        NoScope,
        Response,
        Action
    }
}
