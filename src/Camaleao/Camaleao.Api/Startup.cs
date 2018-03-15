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
            services.AddMvc();
            InitializeInstances(services);
        }

        private void InitializeInstances(IServiceCollection services) {
            services.Configure<Settings>(options => {
                options.ConnectionString
                    = Configuration.GetSection("Mongo:ConnectionString").Value;
                options.Database
                    = Configuration.GetSection("Mongo:Database").Value;
            });
            services.AddTransient<MockService>();
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
