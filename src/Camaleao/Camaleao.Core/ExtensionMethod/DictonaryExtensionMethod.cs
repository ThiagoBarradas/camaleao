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
                string typeSystem = Convert.ToString(types[propertie.Key]);
                Type type = typeSystem.GetTypeChameleon();
                newProperties.Add(propertie.Key, Convert.ChangeType(propertie.Value, type));
            }

            return newProperties;
        }
    }
}
