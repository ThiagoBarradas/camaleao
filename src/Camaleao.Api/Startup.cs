using Camaleao.Api.Middlewares;
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
using Serilog;
using Camaleao.Application.TemplateAgg.Profiles;
using Camaleao.Application.TemplateAgg.Services;
using Camaleao.Core.Validates;
using Camaleao.Infrastructure.Adapter.Seedwork;
using Camaleao.Infrastructure.Adapter;

namespace Camaleao.Api {
    public partial class Startup {
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables("CAMALEAO_");

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            ConfigureSwaggerService(services);

            services
                .AddMvc()
                .AddJsonOptions(p => {
                    p.SerializerSettings.ContractResolver = new DefaultContractResolver() {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    };
                    p.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            InitializeInstances(services);
        }

        private void InitializeInstances(IServiceCollection services) {

            services.AddSingleton<Settings>(new Settings() {
                ConnectionString = this.Configuration["MongoConnectionString"],
                Database = this.Configuration["MongoDatabase"]
            });

            services.AddTransient<Engine>();

            services.AddSingleton<ILogger>(Log.Logger);
            services.AddTransient<IEngineService, EngineService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IContextService, ContextService>();
            services.AddTransient<IMockService, MockService>();
            services.AddTransient<ICreateTemplateValidate, CreateTemplateValidate>();
            services.AddTransient<IMockApiService, MockApiService>();
            services.AddTransient<ITemplateAppService, TemplateAppService>();
            services.AddTransient<IMockAppService, MockAppService>();


            services.AddSingleton<ITypeAdapterFactory, AutoMapperTypeAdapterFactory>();

            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<IContextRepository, ContextRepository>();
            services.AddScoped<IScriptRepository, ScriptEngineRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMockApiService getService) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            InitializeMapper(app);
            app.UseMiddleware<RequestMappingMock>(getService);
            ConfigureSwagger(app, env);
            app.UseMvc();


        }

        private static void InitializeMapper(IApplicationBuilder app) {
            var serviceProvider = app.ApplicationServices;
            TypeAdapterFactory.SetCurrent((ITypeAdapterFactory)serviceProvider.GetService(typeof(ITypeAdapterFactory)));
        }
    }
}
