using System.Linq;

namespace Camaleao.Api
{
    public static class RouteConfig
    {
        public const string Context = "api/context";
        public const string Script = "api/script";
        public const string Template = "api/template";


        public static bool PathContainsRoutes(string path)
        {
            string[] routes = new string[] { Context, Script, Template };

            return routes.Any(p => path.Contains(p)) || path.Equals("/");
        }
    }
}
