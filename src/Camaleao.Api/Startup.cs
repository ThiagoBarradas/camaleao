﻿using System;
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

namespace Camaleao.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            services.AddTransient<IEngineService, EngineService>();
            services.AddTransient<Engine>();
            services.AddTransient<IMockService, MockService>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ITemplateService, TemplateSevice>();
            
            services.AddSingleton<IConfigurationService>(new ConfigurationService(AppDomain.CurrentDomain.BaseDirectory));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
