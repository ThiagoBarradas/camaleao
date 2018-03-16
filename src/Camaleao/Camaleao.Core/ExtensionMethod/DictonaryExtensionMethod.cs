using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.ExtensionMethod
{
    public static class DictonaryExtensionMethod
    {
        public static Dictionary<string, object> ConvertType(this Dictionary<string, object> properties, Dictionary<string, object> types)
        {
            var newProperties = new Dictionary<string, object>();
            foreach(var propertie in properties)
            {
                object value = null;
                Type type = null;

                if(propertie.Value.GetType() == typeof(JArray))
                {
                    value = propertie.Value;
                }
                else
                {
                    string typeSystem = Convert.ToString(types[propertie.Key]);
                    type = typeSystem.GetTypeChameleon();
                    value = Convert.ChangeType(propertie.Value, type);
                }

                newProperties.Add(propertie.Key, value);
            }

            return newProperties;
        }
    }
}
