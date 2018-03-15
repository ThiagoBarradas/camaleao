using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core
{
    public class Template
    {
        public string Route { get; set; }
        public string Id { get; set; }
        public object Request { get; set; }
        public IList<ResponseSettings> Responses { get; set; }
        public IList<Rule> Rules { get; set; }
    }

    public class ResponseSettings
    {
        public string Id { get; set; }
        public Response Response { get; set; }
    }

    public class Response
    {
        public int StatusCode { get; set; }
        public object Body { get; set; }
    }

    public class Rule
    {
        public string Expression { get; set; }
        public string Response { get; set; }
    }
}
