using AutoMapper;
using Camaleao.Infra.Adaper.Seedwork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Camaleao.Infra.Adaper {
    public class AutoMapperTypeAdapterFactory : ITypeAdapterFactory {

        public AutoMapperTypeAdapterFactory(IServiceProvider provider) {

            var profiles = AppDomain.CurrentDomain.GetAssemblies()
                .Where(p => p.FullName.StartsWith("Camaleao"))
                .SelectMany(p => p.GetTypes())
                .Where(p => p.BaseType == typeof(Profile))
                .ToList();

            Mapper.Initialize(cfg => {

                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
                foreach (var profile in profiles) {
                    cfg.AddProfile(Activator.CreateInstance(profile) as Profile);
                }
            });
        }

        public ITypeAdapter Create() {
            return new AutoMapperTypeAdapter();
        }
    }
}
