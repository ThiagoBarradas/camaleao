using Newtonsoft.Json;
using System;

namespace Camaleao.Core.Entities
{
    public class Variable
    {
        public string Name { get; set; }
        public dynamic Initialize { get; set; }

        public bool Builded { get; set; }

        private string _value = string.Empty;
        public string Value
        {
            get
            {
                if(string.IsNullOrEmpty(_value))
                    _value = Initialize;

                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public string Type { get; set; }

        public void BuildVariable()
        {
            if(!this.Builded)
            {
                if(Initialize == null)
                {
                    Value = GetTypeValue();
                }
                else
                {
                    Value = MapperType();
                }
                Initialize = Value;
                this.Builded = true;
            }
        }

        private string MapperType()
        {
            switch(Initialize?.GetType()?.FullName)
            {
                case "System.Int32":
                case "System.Int64":
                {
                    Type = "integer";
                    return $"{Initialize}";
                }
                case "Newtonsoft.Json.Linq.JArray":
                case "Newtonsoft.Json.Linq.JObject":
                {
                    Type = "object";
                    return $"{JsonConvert.SerializeObject(Initialize)}";
                }
                case "System.Double":
                {
                    Type = "double";
                    return $"{Initialize.ToString().Replace(',', '.')}";
                }
                case "System.Boolean":
                {
                    Type = "bool";
                    return $"{Initialize.ToString().ToLower()}";
                }
                default:
                {
                    Type = "text";
                    return $"'{Initialize}'";
                }
            }
        }

        private string GetTypeValue()
        {
            switch(Type)
            {
                case "text":
                return "''";
                case "integer":
                case "double":
                return "0";
                case "bool":
                return "true";
                case "array":
                return "[]";
                default:
                return "''";
            }
        }

    }
}
