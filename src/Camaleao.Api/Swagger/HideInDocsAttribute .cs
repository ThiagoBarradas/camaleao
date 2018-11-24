using System;

namespace Camaleao.Api.Swagger {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HideInDocsAttribute : Attribute {

    }
}
