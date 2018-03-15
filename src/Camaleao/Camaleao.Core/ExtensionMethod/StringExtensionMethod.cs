using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core.ExtensionMethod
{
    public static class StringExtensionMethod
    {
        public static Type GetTypeChameleon(this string content)
        {
            var type = "System.String";

            if(content == "texto")
                type = "System.String";
            else if(content == "inteiro")
                type = "System.Int32";
            else if(content == "decimal")
                type = "System.Double";

            return Type.GetType(type, false, true);
        }
    }
}
