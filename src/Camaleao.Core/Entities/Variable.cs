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

        //public string GetValue()
        //{
        //    if("text".Equals(this.Type.ToLower()))
        //        return $"'{Value}'";
        //    return Value;
        //}

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
            var aaaa = Initialize?.GetType()?.FullName;
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

        //private string Teste(Object initialize)
        //{
            
        //    var properties = initialize.GetType().GetProperties();
        //    foreach(var propriedade in properties)
        //    {
        //        var name = propriedade.Name;
        //        var type = propriedade.PropertyType;
        //        var valuedsa = propriedade.GetValue(properties);    
        //    }

        //    return "";
        //}

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
