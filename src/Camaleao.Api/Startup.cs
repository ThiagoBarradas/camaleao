using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camaleao.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Camaleao.Core.Services;
using AutoMapper;
using Camaleao.Core.Repository;
using Camaleao.Api.Profilers;
using Camaleao.Core.Services.Interfaces;
using Jint;
using System.IO;
using Newtonsoft.Json.Serialization;
using Camaleao.Api.Middlewares;

namespace Camaleao.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
       

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddJsonOptions(p =>
                {
                    p.SerializerSettings.ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                });
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TemplateProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            InitializeInstances(services);
        }

        private void InitializeInstances(IServiceCollection services)
        {

            services.AddSingleton<Settings>(new Settings()
            {
                ConnectionString = this.Configuration["Mongo:ConnectionString"],
                Database = this.Configuration["Mongo:Database"]
            });

            services.AddTransient<Engine>();

            services.AddTransient<IEngineService, EngineService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IContextService, ContextService>();
            services.AddTransient<IMockService, MockService>();
            services.AddScoped<IGetService, GetService>();
            services.AddScoped<IGetMockService, GetMockService>();

            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<IContextRepository, ContextRepository>();
            services.AddScoped<IScriptRepository, ScriptEngineRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IGetService getService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestMappingMock>(getService);

            app.UseMvc();
        }
    }
}
