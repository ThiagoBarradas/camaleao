using Newtonsoft.Json;
using System;

namespace Camaleao.Core.Entities
{
    public class Variable
    {
        public string Name { get; set; }
        public dynamic Initialize { get; set; }
        public string Value { get; set; }

        public string Type { get; set; }

        public void BuildVariable()
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
        }

        private string MapperType()
        {
            switch(Initialize?.GetType()?.FullName)
            {
                case "System.String":
                return $"'{Initialize}'";
                case "Newtonsoft.Json.Linq.JArray":
                case "Newtonsoft.Json.Linq.JObject":
                return $"{JsonConvert.SerializeObject(Initialize)}";
                case "System.Double":
                return $"{Initialize.ToString().Replace(',', '.')}";
                case "System.Boolean":
                return $"{Initialize.ToString().ToLower()}";
                default:
                return $"{Initialize}";
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
