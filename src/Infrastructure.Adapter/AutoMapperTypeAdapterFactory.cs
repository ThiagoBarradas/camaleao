using AutoMapper;
using Camaleao.Infrastructure.Adapter.Seedwork;
using System;
using System.Linq;

namespace Camaleao.Infrastructure.Adapter {
    public class AutoMapperTypeAdapterFactory : ITypeAdapterFactory {

        public AutoMapperTypeAdapterFactory() {

            var profiles = AppDomain.CurrentDomain.GetAssemblies()
                .Where(p => p.FullName.StartsWith("Camaleao"))
                .SelectMany(p => p.GetTypes())
                .Where(p => p.BaseType == typeof(Profile))
                .ToList();

            Mapper.Initialize(cfg => {

                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;

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
