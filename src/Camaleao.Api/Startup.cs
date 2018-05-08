using Camaleao.Api.Middlewares;
using Camaleao.Api.Profilers;
using Camaleao.Core.Repository;
using Camaleao.Core.Services;
using Camaleao.Core.Services.Interfaces;
using Camaleao.Repository;
using Jint;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace Camaleao.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables("CAMALEAO_");

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
       

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
                ConnectionString = this.Configuration["MongoConnectionString"],
                Database = this.Configuration["MongoDatabase"]
            });

            services.AddTransient<Engine>();

            services.AddTransient<IEngineService, EngineService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IContextService, ContextService>();
            services.AddTransient<IMockService, MockService>();
            services.AddTransient<IMockApiService, MockApiService>();
      

            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<IContextRepository, ContextRepository>();
            services.AddScoped<IScriptRepository, ScriptEngineRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMockApiService getService)
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
