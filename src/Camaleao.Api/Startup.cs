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
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;

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
                    p.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TemplateProfile());
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            ConfigureSwaggerService(services);
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

            services.AddSingleton<ILogger>(Log.Logger);
            services.AddTransient<IEngineService, EngineService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<IContextService, ContextService>();
            services.AddTransient<IMockService, MockService>();
            services.AddTransient<IMockApiService, MockApiService>();
            services.AddTransient<ITemplateAppService, TemplateAppService>();
      

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
            ConfigureSwagger(app, env);
            app.UseMvc();
    
        }

        private void ConfigureSwagger(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs"));
            }

            app.UseSwagger(c => c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value));
        }

        private void ConfigureSwaggerService(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                string basePath = PlatformServices.Default.Application.ApplicationBasePath;
                string moduleName = GetType().GetTypeInfo().Module.Name.Replace(".dll", ".xml");
                string filePath = Path.Combine(basePath, moduleName);
                string readme = File.ReadAllText(Path.Combine(basePath, "docs\\README.md"));

                ApiKeyScheme scheme = Configuration.GetSection("ApiKeyScheme").Get<ApiKeyScheme>();
                options.AddSecurityDefinition("Authentication", scheme);

                Info info = Configuration.GetSection("Info").Get<Info>();
                info.Description = readme;
                options.SwaggerDoc(info.Version, info);

                options.IncludeXmlComments(filePath);
                options.DescribeAllEnumsAsStrings();
                //options.OperationFilter<ExamplesOperationFilter>();
                //options.DocumentFilter<HideInDocsFilter>();
            });
        }
    }
}
