using Flunt.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camaleao.Core
{
    public class RequestMapped
    {

        public bool HasContext(string contextKey)
        {
            return true;
        }

        public bool HasExternalContext(string externalContextKey)
        {
            return true;
        }

        public string GetContext()
        {
            return "";
        }

        public string GetExternalContext()
        {

            return "";
        }

        public string ExtractRulesExpression(string expression) {
            return "";
        }

        public IReadOnlyCollection<Notification> ValidateContract()
        {
            return null;
        }


    }
}
