using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core
{
    public static class Delimiters
    {
        public static string[] ContextVariable()
        {
            return new string[] { "_context.{{", "}}" };
        }

        public static string[] ContextComplexElement()
        {
            return new string[] { "_context.$$", "$$" };
        }

        public static string[] ElementRequest()
        {
            return new string[] { "{{", "}}" };
        }

        public static string[] ComplexElement()
        {
            return new string[] { "$$" };
        }
    }
}


