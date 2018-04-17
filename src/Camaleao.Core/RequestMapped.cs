using Camaleao.Core.Entities;
using Camaleao.Core.ExtensionMethod;
using Camaleao.Core.Mappers;
using Camaleao.Core.Services.Interfaces;
using Flunt.Notifications;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Camaleao.Core
{
    public abstract class RequestMapped
    {
        protected readonly Template template;
        protected readonly IEngineService engineService;
        public RequestMapped(Template template, IEngineService engineService)
        {
            this.template = template;
            this.engineService = engineService;
        }
        public abstract bool HasContext();
        public abstract bool HasExternalContext();
        public abstract string GetContext();
        public abstract string GetExternalContext();
        public abstract string ExtractRulesExpression(string expression);
        public abstract IReadOnlyCollection<Notification> ValidateContract();

        public Template GetTemplate()
        {
            return template;
        }
        public IEngineService GetEngineService()
        {
            return engineService;
        }
    }

    public class PostRequestMapped :RequestMapped
    {

        private readonly JObject _request;
        private readonly Template _template;
        private readonly Dictionary<string, dynamic> _TemplateRequestMapped;
        private readonly Dictionary<string, dynamic> _RequestMapped;
        private readonly IEngineService _engine;



        public PostRequestMapped(Template template, IEngineService engine, JObject request) : base(template, engine)
        {
            _template = template;
            _engine = engine;
            _request = request;
            _TemplateRequestMapped = ((JObject)template.Request.Body).MapperContractFromObject();
            _RequestMapped = request.MapperContractFromObject();

            _engine.LoadRequest(request, "_request");
        }
        public override string ExtractRulesExpression(string expression)
        {
            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this.GetEngineService(),false, ScopeExpression.NoScope),
                new ExtractContextComplexElementExpression(this.GetEngineService(),false,ScopeExpression.NoScope),
                new ExtractElementExpression(this.GetEngineService(),false,ScopeExpression.NoScope),
                new ExtractComplextElementExpression(this.GetEngineService(),false,ScopeExpression.NoScope)
            }, expression);
        }

        public override string GetContext()
        {
            string key = _TemplateRequestMapped.FirstOrDefault(r => r.Value.ToString().Equals("_context")).Key ?? string.Empty;
            if(!string.IsNullOrEmpty(key))
                return _RequestMapped[key];

            return string.Empty;
        }

        public override string GetExternalContext()
        {
            string key = (_TemplateRequestMapped.FirstOrDefault(r => r.Value.ToString().Equals("_context.external")).Key ?? string.Empty);

            if(!string.IsNullOrEmpty(key))
                return _RequestMapped[key];

            return string.Empty;
        }

        public override bool HasContext()
        {
            return !this.GetContext().Equals(string.Empty);
        }

        public override bool HasExternalContext()
        {
            return !this.GetExternalContext().Equals(string.Empty);
        }

        public override IReadOnlyCollection<Notification> ValidateContract()
        {
            List<Notification> notification = new List<Notification>();
            foreach(var request in _RequestMapped ?? new Dictionary<string, dynamic>())
            {
                if(!_TemplateRequestMapped.ContainsKey(request.Key.ClearNavigateProperties()))
                {
                    notification.Add(new Notification($"{request.Key}", "The propertie name don't reflect the contract"));
                    continue;
                }

                if(((string)_TemplateRequestMapped[request.Key.ClearNavigateProperties()]).GetTypeChameleon() != _request.SelectToken(request.Key).GetTypeJson())
                    notification.Add(new Notification($"{request.Key}", "The type of the propertie don't reflect the contract"));
            }

            string externalIdentifier = this.GetExternalContext();

            if(!string.IsNullOrEmpty(externalIdentifier) && !externalIdentifier.IsGuid())
                notification.Add(new Notification("Context", "The external context does not have a valid value"));

            return notification;
        }
    }

    public class GetRequestMapped :RequestMapped
    {
        private readonly IEngineService _engine;
        private readonly Dictionary<int, string> _QueryStringMapped;
        private readonly Template _template;
        public GetRequestMapped(Template template, IEngineService engine, string[] queryString) : base(template, engine)
        {
            this._engine = engine;
            this._QueryStringMapped = MapQueryString(queryString);
            this._template = template;
        }

        private Dictionary<int, string> MapQueryString(string[] queryString)
        {
            Dictionary<int, string> identifier = new Dictionary<int, string>();
            for(int i = 0; i < queryString.Length; i++)
            {
                identifier.Add(i, queryString[i]);
            }
            return identifier;
        }
        public override string ExtractRulesExpression(string expression)
        {
            this._template.Request.GetIdentifierFromQueryString()
                .ForEach(x => expression = expression.Replace(x.Value, $"'{this._QueryStringMapped[x.Key]}'"));

            return new ExtractExpression().Extract(new List<ExtractProperties>()
            {
                new ExtractContextExpression(this.GetEngineService(),false, ScopeExpression.NoScope),
                new ExtractContextComplexElementExpression(this.GetEngineService(),false,ScopeExpression.NoScope),
            }, expression);
        }

        public override string GetContext()
        {
            var key = this._template.Request.GetIdentifierFromQueryString().FirstOrDefault(p => p.Value.Equals("_context"));
            if(!key.Equals(default(KeyValuePair<int, string>)) && _QueryStringMapped.ContainsKey(key.Key))
                return _QueryStringMapped[key.Key].ToString();

            return string.Empty;
        }


        public override string GetExternalContext()
        {
            var key = this._template.Request.GetIdentifierFromQueryString().FirstOrDefault(p => p.Value.Equals("_context.external"));
            if(!key.Equals(default(KeyValuePair<int, string>)) && _QueryStringMapped.ContainsKey(key.Key))
                return _QueryStringMapped[key.Key].ToString();

            return string.Empty;
        }

        public override bool HasContext()
        {
            return !string.IsNullOrEmpty(this.GetContext());
        }

        public override bool HasExternalContext()
        {
            return !string.IsNullOrEmpty(this.GetExternalContext());
        }

        public override IReadOnlyCollection<Notification> ValidateContract()
        {
            List<Notification> notification = new List<Notification>();
            foreach(var item in _template.Request.GetIdentifierFromQueryString())
            {

                if(!this._QueryStringMapped.ContainsKey(item.Key))
                {
                    notification.Add(new Notification($"{item.Value}", "The propertie name don't reflect the contract"));
                    continue;
                }
            }
            return notification;
        }
    }
}
